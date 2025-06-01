using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Investments;
using MediatR;

namespace BudgetFlow.Application.Statistics.Queries.GetLastInvestments;
public class GetLastInvestmentsQuery : IRequest<Result<List<InvestmentPaginationResponse>>>
{
    public int PortfolioID { get; set; }
    public class GetLastInvestmentsQueryHandler : IRequestHandler<GetLastInvestmentsQuery, Result<List<InvestmentPaginationResponse>>>
    {
        private readonly IStatisticsRepository _statisticsRepository;
        public GetLastInvestmentsQueryHandler(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }
        public async Task<Result<List<InvestmentPaginationResponse>>> Handle(GetLastInvestmentsQuery request, CancellationToken cancellationToken)
        {
            var investments = await _statisticsRepository.GetLastInvestmentsAsync(request.PortfolioID);
            return Result.Success(investments);
        }
    }
}
