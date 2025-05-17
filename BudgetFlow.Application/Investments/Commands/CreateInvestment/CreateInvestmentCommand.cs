using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Investments.Commands.CreateInvestment;
public class CreateInvestmentCommand : IRequest<Result<bool>>
{
    public InvestmentDto Investment { get; set; }
    public class CreateInvestmentCommandHandler : IRequestHandler<CreateInvestmentCommand, Result<bool>>
    {
        private readonly IInvestmentRepository investmentRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IUserWalletRepository userWalletRepository;
        private readonly IAssetRepository assetRepository;
        private readonly IPortfolioRepository portfolioRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUnitOfWork unitOfWork;

        public CreateInvestmentCommandHandler(
            IInvestmentRepository investmentRepository,
            IWalletRepository walletRepository,
            IUserWalletRepository userWalletRepository,
            IAssetRepository assetRepository,
            IPortfolioRepository portfolioRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            this.investmentRepository = investmentRepository;
            this.walletRepository = walletRepository;
            this.userWalletRepository = userWalletRepository;
            this.assetRepository = assetRepository;
            this.portfolioRepository = portfolioRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(CreateInvestmentCommand request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var portfolio = await portfolioRepository.GetPortfolioByIdAsync(request.Investment.PortfolioId);
            if (portfolio is null)
                return Result.Failure<bool>(PortfolioErrors.PortfolioNotFound);

            var wallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(portfolio.WalletID, userID);
            if (wallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);

            var asset = await assetRepository.GetAssetAsync(request.Investment.AssetId);
            var investment = new Investment
            {
                AssetId = request.Investment.AssetId,
                UnitAmount = request.Investment.UnitAmount,
                Description = request.Investment.Description,
                Date = DateTime.SpecifyKind(request.Investment.Date, DateTimeKind.Utc),
                PortfolioId = request.Investment.PortfolioId,
                Type = request.Investment.Type
            };

            investment.CurrencyAmount = investment.Type == InvestmentType.Buy
                ? investment.UnitAmount * asset.BuyPrice
                : investment.UnitAmount * asset.SellPrice;

            var walletAsset = await walletRepository.GetWalletAssetAsync(portfolio.WalletID, investment.AssetId);

            await unitOfWork.BeginTransactionAsync();
            try
            {
                if (walletAsset is not null)
                {
                    if (investment.Type == InvestmentType.Buy)
                    {
                        if (wallet.Wallet.Balance < investment.CurrencyAmount)
                            return Result.Failure<bool>(WalletErrors.InsufficientBalance);

                        walletAsset.Amount += investment.UnitAmount;
                        walletAsset.Balance = walletAsset.Amount * asset.SellPrice;

                        var walletAssetUpdate = await walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance, saveChanges: false);
                        var walletUpdate = await walletRepository.UpdateWalletAsync(userID, -investment.CurrencyAmount, saveChanges: false);
                    }
                    else
                    {
                        if (walletAsset.Amount < investment.UnitAmount || walletAsset.Balance < investment.CurrencyAmount)
                            return Result.Failure<bool>(WalletAssetErrors.NotEnoughAssetAmount);

                        walletAsset.Amount -= investment.UnitAmount;
                        walletAsset.Balance = walletAsset.Amount * asset.SellPrice;

                        var walletAssetUpdate = await walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance, saveChanges: false);
                        var walletUpdate = await walletRepository.UpdateWalletAsync(userID, investment.CurrencyAmount, saveChanges: false);
                    }
                }
                else
                {
                    if (investment.Type == InvestmentType.Buy)
                    {
                        if (wallet.Wallet.Balance < investment.CurrencyAmount)
                            return Result.Failure<bool>(WalletErrors.InsufficientBalance);

                        var createResult = await walletRepository.CreateWalletAssetAsync(new WalletAsset
                        {
                            WalletId = portfolio.WalletID,
                            AssetId = investment.AssetId,
                            Amount = investment.UnitAmount,
                            Balance = investment.CurrencyAmount
                        }, saveChanges: false);

                        var walletUpdate = await walletRepository.UpdateWalletAsync(userID, -investment.CurrencyAmount, saveChanges: false);
                    }
                    else
                    {
                        return Result.Failure<bool>(WalletErrors.NoBalanceForAsset);
                    }
                }

                var investmentResult = await investmentRepository.CreateInvestmentAsync(investment, saveChanges: false);

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