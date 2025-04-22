using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Investments.Queries;
public class GetPortfolioAssetsQuery : IRequest<Result<PortfolioAssetResponse>>
{
    public string Portfolio { get; set; }
    public GetPortfolioAssetsQuery(string Portfolio)
    {
        this.Portfolio = Portfolio;
    }
    public class GetPortfolioAssetsQueryHandler : IRequestHandler<GetPortfolioAssetsQuery, Result<PortfolioAssetResponse>>
    {
        private readonly IInvestmentRepository investmentRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetPortfolioAssetsQueryHandler(IInvestmentRepository investmentRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.investmentRepository = investmentRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<PortfolioAssetResponse>> Handle(GetPortfolioAssetsQuery request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var investments = await investmentRepository.GetAssetInvestmentsAsync(request.Portfolio, userID);

            return investments != null
                ? Result.Success(investments)
                : Result.Failure<PortfolioAssetResponse>("Failed to get Investments");
        }
    }
}
