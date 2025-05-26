using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Investments.Commands.DeleteInvestment;
public class DeleteInvestmentCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public DeleteInvestmentCommand(int ID)
    {
        this.ID = ID;
    }
    public class DeleteInvestmentCommandHandler : IRequestHandler<DeleteInvestmentCommand, Result<bool>>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrencyRateRepository _currencyRateRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteInvestmentCommandHandler(
            IInvestmentRepository investmentRepository,
            IUserWalletRepository userWalletRepository,
            ICurrentUserService currentUserService,
            IPortfolioRepository portfolioRepository,
            IAssetRepository assetRepository,
            IWalletRepository walletRepository,
            ICurrencyRateRepository currencyRateRepository,
            IUnitOfWork unitOfWork)
        {
            _investmentRepository = investmentRepository;
            _userWalletRepository = userWalletRepository;
            _currentUserService = currentUserService;
            _portfolioRepository = portfolioRepository;
            _assetRepository = assetRepository;
            _walletRepository = walletRepository;
            _currencyRateRepository = currencyRateRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(DeleteInvestmentCommand request, CancellationToken cancellationToken)
        {
            if (request.ID <= 0)
                return Result.Failure<bool>(InvestmentErrors.InvalidInvestmentId);

            var userID = _currentUserService.GetCurrentUserID();

            var investment = await _investmentRepository.GetInvestmentByIdAsync(request.ID);
            if (investment is null)
                return Result.Failure<bool>(InvestmentErrors.InvestmentNotFound);

            var portfolio = await _portfolioRepository.GetPortfolioByIdAsync(investment.PortfolioID);
            if (portfolio is null)
                return Result.Failure<bool>(PortfolioErrors.PortfolioNotFound);

            var wallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(portfolio.WalletID, userID);
            if (wallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);

            var asset = await _assetRepository.GetAssetAsync(investment.AssetID);
            if (asset is null)
                return Result.Failure<bool>(AssetErrors.AssetNotFound);

            investment.CurrencyAmount = investment.Type == InvestmentType.Buy
                ? investment.UnitAmount * asset.BuyPrice
                : investment.UnitAmount * asset.SellPrice;

            var walletAsset = await _walletRepository.GetWalletAssetAsync(portfolio.WalletID, investment.AssetID);
            if (walletAsset is null)
                return Result.Failure<bool>(WalletAssetErrors.NotFound);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (investment.Type == InvestmentType.Buy)
                {
                    if (walletAsset.Amount < investment.UnitAmount)
                        return Result.Failure<bool>(WalletAssetErrors.NotEnoughAssetAmount);

                    // Satın alma siliniyor: varlık azaltılır, para geri verilir
                    walletAsset.Amount -= investment.UnitAmount;
                    walletAsset.Balance -= investment.CurrencyAmount;

                    var assetUpdate = await _walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance, saveChanges: false);
                    if (!assetUpdate)
                        return Result.Failure<bool>(WalletAssetErrors.UserAssetUpdateFailed);

                    var walletUpdate = await _walletRepository.UpdateWalletAsync(portfolio.WalletID, investment.CurrencyAmount, investment.AmountInTRY, saveChanges: false);
                    if (!walletUpdate)
                        return Result.Failure<bool>(WalletErrors.UpdateFailed);
                }
                else
                {
                    // Satış siliniyor: varlık artırılır, para geri alınır
                    if (wallet.Wallet.Balance < investment.CurrencyAmount)
                        return Result.Failure<bool>(WalletErrors.InsufficientBalance);

                    walletAsset.Amount += investment.UnitAmount;
                    walletAsset.Balance = walletAsset.Amount * asset.SellPrice;

                    var assetUpdate = await _walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance, saveChanges: false);
                    if (!assetUpdate)
                        return Result.Failure<bool>(WalletAssetErrors.UserAssetUpdateFailed);

                    var walletUpdate = await _walletRepository.UpdateWalletAsync(portfolio.WalletID, -investment.CurrencyAmount, -investment.AmountInTRY, saveChanges: false);
                    if (!walletUpdate)
                        return Result.Failure<bool>(WalletErrors.UpdateFailed);
                }

                var deleteResult = await _investmentRepository.DeleteInvestmentAsync(request.ID);
                if (!deleteResult)
                    return Result.Failure<bool>(InvestmentErrors.InvestmentDeletionFailed);

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
