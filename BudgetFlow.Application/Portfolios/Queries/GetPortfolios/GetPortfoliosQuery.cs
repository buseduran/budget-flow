using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Portfolios.Queries.GetPortfolios
{
    public class GetPortfoliosQuery : IRequest<List<PortfolioResponse>>
    {
        public class GetPortfoliosQueryHandler : IRequestHandler<GetPortfoliosQuery, List<PortfolioResponse>>
        {
            private readonly IPortfolioRepository portfolioRepository;
            private readonly IHttpContextAccessor httpContextAccessor;
            public GetPortfoliosQueryHandler(IPortfolioRepository portfolioRepository, IHttpContextAccessor httpContextAccessor)
            {
                this.portfolioRepository = portfolioRepository;
                this.httpContextAccessor = httpContextAccessor;
            }
            public async Task<List<PortfolioResponse>> Handle(GetPortfoliosQuery request, CancellationToken cancellationToken)
            {
                GetCurrentUser getCurrentUser = new(httpContextAccessor);
                int UserID = getCurrentUser.GetCurrentUserID();

                return await portfolioRepository.GetPortfoliosAsync(UserID);
            }
        }
    }
}
