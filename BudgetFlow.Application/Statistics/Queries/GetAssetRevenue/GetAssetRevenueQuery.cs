using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Statistics.Responses;
using MediatR;

namespace BudgetFlow.Application.Statistics.Queries.GetAssetRevenue;

public record GetAssetRevenueQuery(string Portfolio) : IRequest<Result<List<AssetRevenueResponse>>>;

public class GetAssetRevenueQueryHandler : IRequestHandler<GetAssetRevenueQuery, Result<List<AssetRevenueResponse>>>
{
    private readonly IStatisticsRepository _statisticsRepository;

    public GetAssetRevenueQueryHandler(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }

    public async Task<Result<List<AssetRevenueResponse>>> Handle(GetAssetRevenueQuery request, CancellationToken cancellationToken)
    {
        var result = await _statisticsRepository.GetAssetRevenueAsync(request.Portfolio);
        return Result.Success(result);
    }
}