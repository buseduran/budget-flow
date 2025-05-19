using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

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
        private readonly IInvestmentRepository investmentRepository;
        private readonly IAssetRepository assetRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IPortfolioRepository portfolioRepository;
        private readonly IUserWalletRepository userWalletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUnitOfWork unitOfWork;

        public UpdateInvestmentCommandHandler(
            IInvestmentRepository investmentRepository,
            IAssetRepository assetRepository,
            IWalletRepository walletRepository,
            IPortfolioRepository portfolioRepository,
            IUserWalletRepository userWalletRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            this.investmentRepository = investmentRepository;
            this.assetRepository = assetRepository;
            this.walletRepository = walletRepository;
            this.portfolioRepository = portfolioRepository;
            this.userWalletRepository = userWalletRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateInvestmentCommand request, CancellationToken cancellationToken)
        {
            if (request.ID <= 0)
                return Result.Failure<bool>(InvestmentErrors.InvalidInvestmentId);

            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            var existingInvestment = await investmentRepository.GetInvestmentByIdAsync(request.ID);
            if (existingInvestment is null)
                return Result.Failure<bool>(InvestmentErrors.InvestmentNotFound);

            var portfolio = await portfolioRepository.GetPortfolioByIdAsync(existingInvestment.PortfolioID);
            if (portfolio is null)
                return Result.Failure<bool>(PortfolioErrors.PortfolioNotFound);

            var userWallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(portfolio.WalletID, userID);
            if (userWallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);

            var asset = await assetRepository.GetAssetAsync(existingInvestment.AssetID);
            if (asset is null)
                return Result.Failure<bool>(AssetErrors.AssetNotFound);

            var type = existingInvestment.Type;

            decimal newCurrencyAmount = type == InvestmentType.Buy
                ? request.UnitAmount * asset.BuyPrice
                : request.UnitAmount * asset.SellPrice;

            var walletAsset = await walletRepository.GetWalletAssetAsync(portfolio.WalletID, existingInvestment.AssetID);
            if (walletAsset is null)
                return Result.Failure<bool>(WalletAssetErrors.NotEnoughAssetAmount);

            decimal unitDifference = request.UnitAmount - existingInvestment.UnitAmount;
            decimal balanceDifference = newCurrencyAmount - existingInvestment.CurrencyAmount;

            await unitOfWork.BeginTransactionAsync();

            try
            {
                if (type == InvestmentType.Buy)
                {
                    if (userWallet.Wallet.Balance < balanceDifference)
                        return Result.Failure<bool>(WalletErrors.InsufficientBalance);

                    walletAsset.Amount += unitDifference;
                    walletAsset.Balance += balanceDifference;

                    await walletRepository.UpdateWalletAsync(userWallet.Wallet.ID, -balanceDifference, saveChanges: false);
                }
                else
                {
                    if (walletAsset.Amount - unitDifference < 0)
                        return Result.Failure<bool>(WalletAssetErrors.NotEnoughAssetAmount);

                    walletAsset.Amount -= unitDifference;
                    walletAsset.Balance -= balanceDifference;

                    await walletRepository.UpdateWalletAsync(userWallet.Wallet.ID, balanceDifference, saveChanges: false);
                }

                await walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance, saveChanges: false);

                existingInvestment.Description = request.Description;
                existingInvestment.UnitAmount = request.UnitAmount;
                existingInvestment.CurrencyAmount = newCurrencyAmount;
                existingInvestment.Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc);

                var investmentDto = new InvestmentDto
                {
                    UnitAmount = request.UnitAmount,
                    CurrencyAmount = newCurrencyAmount,
                    Description = request.Description,
                    Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
                };

                var updateResult = await investmentRepository.UpdateInvestmentAsync(request.ID, investmentDto);
                if (!updateResult)
                    return Result.Failure<bool>(InvestmentErrors.InvestmentUpdateFailed);

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
