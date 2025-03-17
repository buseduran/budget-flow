using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Investments.Queries.GetAssetRevenue
{
    public class GetAssetRevenueQuery : IRequest<List<Dictionary<string, object>>>
    {
        public string Portfolio { get; set; }
        public GetAssetRevenueQuery(string Portfolio)
        {
            this.Portfolio = Portfolio;
        }
        public class GetAssetRevenueQueryHandler : IRequestHandler<GetAssetRevenueQuery, List<Dictionary<string, object>>>
        {
            private readonly IInvestmentRepository investmentRepository;
            public GetAssetRevenueQueryHandler(IInvestmentRepository investmentRepository)
            {
                this.investmentRepository = investmentRepository;
            }

            public async Task<List<Dictionary<string, object>>> Handle(GetAssetRevenueQuery request, CancellationToken cancellationToken)
            {
                var result = await investmentRepository.GetAssetRevenueAsync(request.Portfolio);
                if (result is null)
                {
                    throw new Exception("No data found");
                }
                return result;
            }
        }
    }
}
