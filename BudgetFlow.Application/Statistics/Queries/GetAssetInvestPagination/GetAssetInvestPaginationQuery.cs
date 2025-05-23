using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Statistics.Responses;
using MediatR;

namespace BudgetFlow.Application.Statistics.Queries.GetAssetInvestPagination;

public record GetAssetInvestPaginationQuery : IRequest<Result<PaginatedAssetInvestResponse>>
{
    public int PortfolioID { get; set; }
    public int AssetID { get; set; }
    public int WalletID { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool ConvertToTRY { get; set; } = false;
}

public class GetAssetInvestPaginationQueryHandler : IRequestHandler<GetAssetInvestPaginationQuery, Result<PaginatedAssetInvestResponse>>
{
    private readonly IStatisticsRepository _statisticsRepository;

    public GetAssetInvestPaginationQueryHandler(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }

    public async Task<Result<PaginatedAssetInvestResponse>> Handle(GetAssetInvestPaginationQuery request, CancellationToken cancellationToken)
    {
        var result = await _statisticsRepository.GetAssetInvestsPaginationAsync(request);
        return Result.Success(result);
    }
}