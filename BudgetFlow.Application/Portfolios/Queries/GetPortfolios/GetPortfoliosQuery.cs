using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Portfolios.Queries.GetPortfolios;
public class GetPortfoliosQuery : IRequest<Result<List<PortfolioResponse>>>
{
    public class GetPortfoliosQueryHandler : IRequestHandler<GetPortfoliosQuery, Result<List<PortfolioResponse>>>
    {
        private readonly IPortfolioRepository portfolioRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetPortfoliosQueryHandler(IPortfolioRepository portfolioRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.portfolioRepository = portfolioRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<List<PortfolioResponse>>> Handle(GetPortfoliosQuery request, CancellationToken cancellationToken)
        {
            GetCurrentUser getCurrentUser = new(httpContextAccessor);
            int UserID = getCurrentUser.GetCurrentUserID();

            var result = await portfolioRepository.GetPortfoliosAsync(UserID);
            return result != null
                ? Result.Success(result)
                : Result.Failure<List<PortfolioResponse>>("Portfolios not found");
        }
    }
}
