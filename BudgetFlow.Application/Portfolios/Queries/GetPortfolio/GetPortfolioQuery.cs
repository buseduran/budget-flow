using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Portfolios.Queries.GetPortfolio
{
    public class GetPortfolioQuery : IRequest<PortfolioResponse>
    {
        public string Name { get; set; }
        public GetPortfolioQuery(string Name)
        {
            this.Name = Name;
        }
        public class GetPortfolioQueryHandler:IRequestHandler<GetPortfolioQuery, PortfolioResponse>
        {
            private readonly IPortfolioRepository portfolioRepository;
            public GetPortfolioQueryHandler(IPortfolioRepository portfolioRepository)
            {
                this.portfolioRepository = portfolioRepository;
            }
            public async Task<PortfolioResponse> Handle(GetPortfolioQuery request, CancellationToken cancellationToken)
            {
                var portfolio = await portfolioRepository.GetPortfolioAsync(request.Name);
                if (portfolio == null)
                {
                    throw new Exception("Portfolio not found");
                }
                return portfolio;
            }
        }
    }
}
