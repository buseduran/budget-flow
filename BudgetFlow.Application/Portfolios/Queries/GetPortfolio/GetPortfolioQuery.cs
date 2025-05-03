using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Portfolios.Queries.GetPortfolio;
public class GetPortfolioQuery : IRequest<Result<PortfolioResponse>>
{
    public string Name { get; set; }
    public GetPortfolioQuery(string Name)
    {
        this.Name = Name;
    }
    public class GetPortfolioQueryHandler : IRequestHandler<GetPortfolioQuery, Result<PortfolioResponse>>
    {
        private readonly IPortfolioRepository portfolioRepository;
        public GetPortfolioQueryHandler(IPortfolioRepository portfolioRepository)
        {
            this.portfolioRepository = portfolioRepository;
        }
        public async Task<Result<PortfolioResponse>> Handle(GetPortfolioQuery request, CancellationToken cancellationToken)
        {
            var portfolio = await portfolioRepository.GetPortfolioAsync(request.Name);
            return portfolio != null
                ? Result.Success(portfolio)
                : Result.Failure<PortfolioResponse>(PortfolioErrors.PortfolioNotFound);
        }
    }
}
