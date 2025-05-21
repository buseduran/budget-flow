using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

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
        private readonly IInvestmentRepository investmentRepository;
        private readonly IUserWalletRepository userWalletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IPortfolioRepository portfolioRepository;
        private readonly IAssetRepository assetRepository;
        private readonly IWalletRepository walletRepository;
        private readonly ICurrencyRateRepository currencyRateRepository;
        private readonly IUnitOfWork unitOfWork;
        public DeleteInvestmentCommandHandler(
            IInvestmentRepository investmentRepository,
            IUserWalletRepository userWalletRepository,
            IHttpContextAccessor httpContextAccessor,
            IPortfolioRepository portfolioRepository,
            IAssetRepository assetRepository,
            IWalletRepository walletRepository,
            ICurrencyRateRepository currencyRateRepository,
            IUnitOfWork unitOfWork)
        {
            this.investmentRepository = investmentRepository;
            this.userWalletRepository = userWalletRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.portfolioRepository = portfolioRepository;
            this.assetRepository = assetRepository;
            this.walletRepository = walletRepository;
            this.currencyRateRepository = currencyRateRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(DeleteInvestmentCommand request, CancellationToken cancellationToken)
        {
            if (request.ID <= 0)
                return Result.Failure<bool>(InvestmentErrors.InvalidInvestmentId);

            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            var investment = await investmentRepository.GetInvestmentByIdAsync(request.ID);
            if (investment is null)
                return Result.Failure<bool>(InvestmentErrors.InvestmentNotFound);

            var portfolio = await portfolioRepository.GetPortfolioByIdAsync(investment.PortfolioID);
            if (portfolio is null)
                return Result.Failure<bool>(PortfolioErrors.PortfolioNotFound);

            var wallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(portfolio.WalletID, userID);
            if (wallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);

            var asset = await assetRepository.GetAssetAsync(investment.AssetID);
            if (asset is null)
                return Result.Failure<bool>(AssetErrors.AssetNotFound);

            investment.CurrencyAmount = investment.Type == InvestmentType.Buy
                ? investment.UnitAmount * asset.BuyPrice
                : investment.UnitAmount * asset.SellPrice;

            #region AmountInTRY hesapla
            var currency = wallet.Wallet.Currency;
            decimal exchangeRateToTRY = 1m;

            if (currency != CurrencyType.TRY)
            {
                var currencyRate = await currencyRateRepository.GetCurrencyRateByType(currency);
                exchangeRateToTRY = currencyRate.ForexSelling;
            }

            investment.AmountInTRY = investment.CurrencyAmount * exchangeRateToTRY;
            #endregion

            var walletAsset = await walletRepository.GetWalletAssetAsync(portfolio.WalletID, investment.AssetID);
            if (walletAsset is null)
                return Result.Failure<bool>(WalletAssetErrors.NotFound);

            await unitOfWork.BeginTransactionAsync();
            try
            {
                if (investment.Type == InvestmentType.Buy)
                {
                    if (walletAsset.Amount < investment.UnitAmount)
                        return Result.Failure<bool>(WalletAssetErrors.NotEnoughAssetAmount);

                    // Satın alma siliniyor: varlık azaltılır, para geri verilir
                    walletAsset.Amount -= investment.UnitAmount;
                    walletAsset.Balance -= investment.CurrencyAmount;

                    var assetUpdate = await walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance, saveChanges: false);
                    if (!assetUpdate)
                        return Result.Failure<bool>(WalletAssetErrors.UserAssetUpdateFailed);

                    var walletUpdate = await walletRepository.UpdateWalletAsync(portfolio.WalletID, investment.CurrencyAmount, investment.AmountInTRY, saveChanges: false);
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

                    var assetUpdate = await walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance, saveChanges: false);
                    if (!assetUpdate)
                        return Result.Failure<bool>(WalletAssetErrors.UserAssetUpdateFailed);

                    var walletUpdate = await walletRepository.UpdateWalletAsync(portfolio.WalletID, -investment.CurrencyAmount, -investment.AmountInTRY, saveChanges: false);
                    if (!walletUpdate)
                        return Result.Failure<bool>(WalletErrors.UpdateFailed);
                }

                var deleteResult = await investmentRepository.DeleteInvestmentAsync(request.ID);
                if (!deleteResult)
                    return Result.Failure<bool>(InvestmentErrors.InvestmentDeletionFailed);

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                return Result.Failure<bool>(GeneralErrors.FromMessage(ex.Message));
            }
        }
    }
}
