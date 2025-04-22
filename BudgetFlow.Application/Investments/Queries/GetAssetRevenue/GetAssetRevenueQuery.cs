using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Investments.Queries.GetAssetRevenue;
public class GetAssetRevenueQuery : IRequest<Result<List<Dictionary<string, object>>>>
{
    public string Portfolio { get; set; }
    public GetAssetRevenueQuery(string Portfolio)
    {
        this.Portfolio = Portfolio;
    }
    public class GetAssetRevenueQueryHandler : IRequestHandler<GetAssetRevenueQuery, Result<List<Dictionary<string, object>>>>
    {
        private readonly IInvestmentRepository investmentRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetAssetRevenueQueryHandler(IInvestmentRepository investmentRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.investmentRepository = investmentRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<List<Dictionary<string, object>>>> Handle(GetAssetRevenueQuery request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            var result = await investmentRepository.GetAssetRevenueAsync(request.Portfolio, userID);
            return result != null
                ? Result.Success(result)
                : Result.Failure<List<Dictionary<string, object>>>("No data found"); ;
        }
    }
}
