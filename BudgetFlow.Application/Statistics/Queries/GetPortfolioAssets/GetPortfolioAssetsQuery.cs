using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Statistics.Queries.GetPortfolioAssets;
public class GetPortfolioAssetsQuery : IRequest<Result<PortfolioAssetResponse>>
{
    public int PortfolioID { get; set; }
    public GetPortfolioAssetsQuery(int PortfolioID)
    {
        this.PortfolioID = PortfolioID;
    }
    public class GetPortfolioAssetsQueryHandler : IRequestHandler<GetPortfolioAssetsQuery, Result<PortfolioAssetResponse>>
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetPortfolioAssetsQueryHandler(
            IStatisticsRepository statisticsRepository,
            ICurrentUserService currentUserService)
        {
            _statisticsRepository = statisticsRepository;
            _currentUserService = currentUserService;
        }
        public async Task<Result<PortfolioAssetResponse>> Handle(GetPortfolioAssetsQuery request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var investments = await _statisticsRepository.GetAssetInvestmentsAsync(request.PortfolioID, userID);

            return investments != null
                ? Result.Success(investments)
                : Result.Failure<PortfolioAssetResponse>(InvestmentErrors.InvestmentNotFound);
        }
    }
}
