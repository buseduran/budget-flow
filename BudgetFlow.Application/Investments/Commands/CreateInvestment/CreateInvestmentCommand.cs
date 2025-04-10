using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Investments.Commands.CreateInvestment
{
    public class CreateInvestmentCommand : IRequest<bool>
    {
        public InvestmentDto Investment { get; set; }
        public class CreateInvestmentCommandHandler : IRequestHandler<CreateInvestmentCommand, bool>
        {
            private readonly IInvestmentRepository investmentRepository;
            private readonly IWalletRepository walletRepository;
            private readonly IAssetRepository assetRepository;
            private readonly IHttpContextAccessor httpContextAccessor;
            public CreateInvestmentCommandHandler(IInvestmentRepository investmentRepository, IWalletRepository walletRepository, IAssetRepository assetRepository, IHttpContextAccessor httpContextAccessor)
            {
                this.investmentRepository = investmentRepository;
                this.walletRepository = walletRepository;
                this.assetRepository = assetRepository;
                this.httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(CreateInvestmentCommand request, CancellationToken cancellationToken)
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

                GetCurrentUser getCurrentUser = new(httpContextAccessor);
                var UserAsset = await assetRepository.GetUserAssetAsync(getCurrentUser.GetCurrentUserID(), investment.AssetId);
                var Wallet = await walletRepository.GetWalletAsync(getCurrentUser.GetCurrentUserID());
                if (UserAsset is not null)
                {
                    if (investment.Type == InvestmentType.Buy)
                    {
                        if (Wallet.Balance > investment.CurrencyAmount)
                        {
                            UserAsset.Amount += investment.UnitAmount;
                            UserAsset.Balance += investment.CurrencyAmount;
                            var walletUpdateResult = await walletRepository.UpdateWalletAsync(getCurrentUser.GetCurrentUserID(), investment.CurrencyAmount);
                            if (!walletUpdateResult)
                            {
                                throw new Exception("Wallet could not be updated.");
                            }
                        }
                    }
                    else
                    {
                        if (UserAsset.Amount < investment.UnitAmount)
                        {
                            throw new Exception("Not enough asset amount.");
                        }
                        UserAsset.Amount -= investment.UnitAmount;
                        UserAsset.Balance -= investment.CurrencyAmount;
                        var walletUpdateResult = await walletRepository.UpdateWalletAsync(getCurrentUser.GetCurrentUserID(), -investment.CurrencyAmount);
                        if (!walletUpdateResult)
                        {
                            throw new Exception("Wallet could not be updated.");
                        }
                    }
                }
                else
                {
                    if (investment.Type == InvestmentType.Buy)
                    {
                        if (Wallet.Balance > investment.CurrencyAmount)
                        {
                            var result = await assetRepository.CreateUserAssetAsync(new UserAsset
                            {
                                UserId = getCurrentUser.GetCurrentUserID(),
                                AssetId = investment.AssetId,
                                Amount = investment.Type == InvestmentType.Buy ? investment.UnitAmount : -investment.UnitAmount,
                                Balance = investment.Type == InvestmentType.Buy ? investment.CurrencyAmount : -investment.CurrencyAmount
                            });
                            if (!result)
                            {
                                throw new Exception("User asset could not be created.");
                            }
                            var walletUpdateResult = await walletRepository.UpdateWalletAsync(getCurrentUser.GetCurrentUserID(), -investment.CurrencyAmount);
                            if (!walletUpdateResult)
                            {
                                throw new Exception("Wallet could not be updated.");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("There is no balance for this asset.");
                    }
                }

                var investmentResult = await investmentRepository.CreateInvestmentAsync(investment);
                if (!investmentResult)
                {
                    throw new Exception("Investment could not be saved.");
                }
                #endregion

                return true;
            }
        }
    }
}
