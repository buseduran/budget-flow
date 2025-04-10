using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Investments.Queries
{
    public class GetPortfolioAssetsQuery : IRequest<PortfolioAssetResponse>
    {
        public string Portfolio { get; set; }
        public GetPortfolioAssetsQuery(string Portfolio)
        {
            this.Portfolio = Portfolio;
        }
        public class GetPortfolioAssetsQueryHandler : IRequestHandler<GetPortfolioAssetsQuery, PortfolioAssetResponse>
        {
            private readonly IInvestmentRepository investmentRepository;
            public GetPortfolioAssetsQueryHandler(IInvestmentRepository investmentRepository)
            {
                this.investmentRepository = investmentRepository;
            }
            public async Task<PortfolioAssetResponse> Handle(GetPortfolioAssetsQuery request, CancellationToken cancellationToken)
            {
                var investments = await investmentRepository.GetAssetInvestmentsAsync(request.Portfolio);
                return investments;
            }
        }
    }
}
