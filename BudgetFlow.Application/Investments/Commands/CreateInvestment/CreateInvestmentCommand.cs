using BudgetFlow.Application.Common.Dtos;
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
        public CreateInvestmentCommandHandler(
            IInvestmentRepository investmentRepository,
            IWalletRepository walletRepository,
            IUserWalletRepository userWalletRepository,
            IAssetRepository assetRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            this.investmentRepository = investmentRepository;
            this.walletRepository = walletRepository;
            this.userWalletRepository = userWalletRepository;
            this.assetRepository = assetRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<bool>> Handle(CreateInvestmentCommand request, CancellationToken cancellationToken)
        {
            #region Save Investment
            var investment = new Investment
            {
                AssetId = request.Investment.AssetId,
                CurrencyAmount = request.Investment.CurrencyAmount,
                UnitAmount = request.Investment.UnitAmount,
                Description = request.Investment.Description,
                Date = DateTime.SpecifyKind(request.Investment.Date, DateTimeKind.Utc),
                PortfolioId = request.Investment.PortfolioId,
                Type = request.Investment.Type
            };
            var asset = await assetRepository.GetAssetAsync(investment.AssetId);
            investment.CurrencyAmount = investment.Type == InvestmentType.Buy
                                        ? investment.UnitAmount * asset.BuyPrice
                                        : investment.UnitAmount * asset.SellPrice;

            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var portfolio = await portfolioRepository.GetPortfolioByIdAsync(request.Investment.PortfolioId);
            var walletAsset = await walletRepository.GetWalletAssetAsync(portfolio.WalletID, investment.AssetId);

            var wallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(portfolio.WalletID, userID);
            if (wallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);

            if (walletAsset is not null)
            {
                if (investment.Type == InvestmentType.Buy)
                {
                    if (wallet.Wallet.Balance > investment.CurrencyAmount)
                    {
                        walletAsset.Amount += investment.UnitAmount;
                        walletAsset.Balance += investment.CurrencyAmount;
                        
                        var walletAssetResult = await walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance);
                        if (!walletAssetResult)
                        {
                            return Result.Failure<bool>(WalletAssetErrors.UserAssetUpdateFailed);
                        }

                        var walletUpdateResult = await walletRepository.UpdateWalletAsync(userID, investment.CurrencyAmount);
                        if (!walletUpdateResult)
                        {
                            return Result.Failure<bool>(WalletErrors.UpdateFailed);
                        }
                    }
                }
                else
                {
                    if (walletAsset.Amount < investment.UnitAmount)
                    {
                        return Result.Failure<bool>(WalletAssetErrors.NotEnoughAssetAmount);
                    }
                    walletAsset.Amount -= investment.UnitAmount;
                    walletAsset.Balance -= investment.CurrencyAmount;
                    var userAssetResult = await walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance);
                    if (!userAssetResult)
                    {
                        return Result.Failure<bool>(WalletAssetErrors.UserAssetUpdateFailed);
                    }

                    var walletUpdateResult = await walletRepository.UpdateWalletAsync(userID, -investment.CurrencyAmount);
                    if (!walletUpdateResult)
                    {
                        return Result.Failure<bool>(WalletErrors.UpdateFailed);
                    }
                }
            }
            else
            {
                if (investment.Type == InvestmentType.Buy)
                {
                    if (wallet.Wallet.Balance > investment.CurrencyAmount)
                    {
                        var result = await walletRepository.CreateWalletAssetAsync(new WalletAsset
                        {
                            WalletId = portfolio.WalletID,
                            AssetId = investment.AssetId,
                            Amount = investment.Type == InvestmentType.Buy ? investment.UnitAmount : -investment.UnitAmount,
                            Balance = investment.Type == InvestmentType.Buy ? investment.CurrencyAmount : -investment.CurrencyAmount
                        });

                        if (!result)
                        {
                            return Result.Failure<bool>(WalletAssetErrors.CreationFailed);
                        }
                        var walletUpdateResult = await walletRepository.UpdateWalletAsync(userID, -investment.CurrencyAmount);
                        if (!walletUpdateResult)
                        {
                            return Result.Failure<bool>(WalletErrors.UpdateFailed);
                        }
                    }
                }
                else
                {
                    return Result.Failure<bool>(WalletErrors.NoBalanceForAsset);
                }
            }

            var investmentResult = await investmentRepository.CreateInvestmentAsync(investment);
            if (!investmentResult)
            {
                return Result.Failure<bool>(InvestmentErrors.CreationFailed);
            }
            #endregion

            return Result.Success(true);
        }
    }
}
