using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

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
            private readonly IHttpContextAccessor httpContextAccessor;
            public GetPortfolioAssetsQueryHandler(IInvestmentRepository investmentRepository, IHttpContextAccessor httpContextAccessor)
            {
                this.investmentRepository = investmentRepository;
                this.httpContextAccessor = httpContextAccessor;
            }
            public async Task<PortfolioAssetResponse> Handle(GetPortfolioAssetsQuery request, CancellationToken cancellationToken)
            {
                var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
                var investments = await investmentRepository.GetAssetInvestmentsAsync(request.Portfolio, userID);

                return investments;
            }
        }
    }
}
