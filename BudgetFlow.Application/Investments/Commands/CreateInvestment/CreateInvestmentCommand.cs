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
                    PortfolioId = request.Investment.PortfolioId
                };
                var asset = await assetRepository.GetAssetAsync(investment.AssetId);
                investment.Price = request.Investment.Type == InvestmentType.Sell ? asset.SellPrice
                      : asset.BuyPrice;

                var investmentResult = await investmentRepository.CreateInvestmentAsync(investment);
                if (!investmentResult)
                {
                    throw new Exception("Investment could not be saved.");
                }
                #endregion

                #region Update Wallet
                investment.Price = request.Investment.CurrencyAmount;

                GetCurrentUser getCurrentUser = new(httpContextAccessor);
                var walletResult = await walletRepository.UpdateWalletAsync(getCurrentUser.GetCurrentUserID(), -investment.CurrencyAmount);
                if (!walletResult)
                {
                    throw new Exception("Wallet could not be updated.");
                }
                #endregion

                return true;
            }
        }
    }
}
