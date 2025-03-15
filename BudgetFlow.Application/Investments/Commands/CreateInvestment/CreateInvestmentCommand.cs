using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
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
                    Amount = request.Investment.Amount,
                    Description = request.Investment.Description,
                    PurchaseDate = request.Investment.PurchaseDate,
                    PortfolioId = request.Investment.PortfolioId,
                };
                var investmentResult = await investmentRepository.CreateInvestmentAsync(investment);
                if (!investmentResult)
                {
                    throw new Exception("Investment could not be saved.");
                }
                #endregion

                #region Update Wallet
                var asset = await assetRepository.GetAssetAsync(investment.AssetId);
                investment.PurchasePrice = asset.CurrentPrice * investment.Amount;

                GetCurrentUser getCurrentUser = new(httpContextAccessor);
                var walletResult = await walletRepository.UpdateWalletAsync(getCurrentUser.GetCurrentUserID(), -investment.Amount);
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
