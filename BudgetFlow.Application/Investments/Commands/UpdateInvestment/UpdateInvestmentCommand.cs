using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Investments.Commands.UpdateInvestment;

public class UpdateInvestmentCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public decimal UnitAmount { get; set; }
    public decimal CurrencyAmount { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }

    public class UpdateInvestmentCommandHandler : IRequestHandler<UpdateInvestmentCommand, Result<bool>>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrencyRateRepository _currencyRateRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateInvestmentCommandHandler(
            IInvestmentRepository investmentRepository,
            IAssetRepository assetRepository,
            IWalletRepository walletRepository,
            IPortfolioRepository portfolioRepository,
            IUserWalletRepository userWalletRepository,
            ICurrentUserService currentUserService,
            ICurrencyRateRepository currencyRateRepository,
            IUnitOfWork unitOfWork)
        {
            _investmentRepository = investmentRepository;
            _assetRepository = assetRepository;
            _walletRepository = walletRepository;
            _portfolioRepository = portfolioRepository;
            _userWalletRepository = userWalletRepository;
            _currentUserService = currentUserService;
            _currencyRateRepository = currencyRateRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateInvestmentCommand request, CancellationToken cancellationToken)
        {
            if (request.ID <= 0)
                return Result.Failure<bool>(InvestmentErrors.InvalidInvestmentId);

            var userID = _currentUserService.GetCurrentUserID();

            var existingInvestment = await _investmentRepository.GetInvestmentByIdAsync(request.ID);
            if (existingInvestment is null)
                return Result.Failure<bool>(InvestmentErrors.InvestmentNotFound);

            var portfolio = await _portfolioRepository.GetPortfolioByIdAsync(existingInvestment.PortfolioID);
            if (portfolio is null)
                return Result.Failure<bool>(PortfolioErrors.PortfolioNotFound);

            var userWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(portfolio.WalletID, userID);
            if (userWallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);

            var asset = await _assetRepository.GetAssetAsync(existingInvestment.AssetID);
            if (asset is null)
                return Result.Failure<bool>(AssetErrors.AssetNotFound);

            var type = existingInvestment.Type;

            decimal newCurrencyAmount = type == InvestmentType.Buy
                ? request.UnitAmount * asset.BuyPrice
                : request.UnitAmount * asset.SellPrice;


            var walletAsset = await _walletRepository.GetWalletAssetAsync(portfolio.WalletID, existingInvestment.AssetID);
            if (walletAsset is null)
                return Result.Failure<bool>(WalletAssetErrors.NotEnoughAssetAmount);

            decimal unitDifference = request.UnitAmount - existingInvestment.UnitAmount;
            decimal balanceDifference = newCurrencyAmount - existingInvestment.CurrencyAmount;

            #region AmountInTRY güncelle
            var currency = userWallet.Wallet.Currency;
            decimal exchangeRateToTRY = 1m;

            if (currency != CurrencyType.TRY)
            {
                var currencyRate = await _currencyRateRepository.GetCurrencyRateByType(currency);
                exchangeRateToTRY = currencyRate.ForexSelling;
            }
            decimal balanceDifferenceInTRY = balanceDifference * exchangeRateToTRY;
            #endregion


            await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (type == InvestmentType.Buy)
                {
                    if (userWallet.Wallet.Balance < balanceDifference)
                        return Result.Failure<bool>(WalletErrors.InsufficientBalance);

                    walletAsset.Amount += unitDifference;
                    walletAsset.Balance += balanceDifference;

                    await _walletRepository.UpdateWalletAsync(userWallet.Wallet.ID, -balanceDifference, -balanceDifferenceInTRY, saveChanges: false);
                }
                else
                {
                    if (walletAsset.Amount - unitDifference < 0)
                        return Result.Failure<bool>(WalletAssetErrors.NotEnoughAssetAmount);

                    walletAsset.Amount -= unitDifference;
                    walletAsset.Balance -= balanceDifference;

                    await _walletRepository.UpdateWalletAsync(userWallet.Wallet.ID, balanceDifference, balanceDifferenceInTRY, saveChanges: false);
                }

                await _walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance, saveChanges: false);

                existingInvestment.Description = request.Description;
                existingInvestment.UnitAmount = request.UnitAmount;
                existingInvestment.CurrencyAmount = newCurrencyAmount;
                existingInvestment.AmountInTRY = newCurrencyAmount * exchangeRateToTRY;
                existingInvestment.ExchangeRate = exchangeRateToTRY;
                existingInvestment.Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc);

                var investmentDto = new InvestmentDto
                {
                    UnitAmount = request.UnitAmount,
                    CurrencyAmount = newCurrencyAmount,
                    AmountInTRY = newCurrencyAmount * exchangeRateToTRY,
                    ExchangeRate = exchangeRateToTRY,
                    Description = request.Description,
                    Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
                };

                var updateResult = await _investmentRepository.UpdateInvestmentAsync(request.ID, investmentDto);
                if (!updateResult)
                    return Result.Failure<bool>(InvestmentErrors.InvestmentUpdateFailed);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure<bool>(GeneralErrors.FromMessage(ex.Message));
            }
        }
    }
}
