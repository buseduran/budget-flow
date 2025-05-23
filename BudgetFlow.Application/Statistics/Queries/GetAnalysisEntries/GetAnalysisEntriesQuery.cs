using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Statistics.Responses;
using MediatR;

namespace BudgetFlow.Application.Statistics.Queries.GetAnalysisEntries;

public record GetAnalysisEntriesQuery : IRequest<Result<AnalysisEntriesResponse>>
{
    public int WalletID { get; set; }
    public string Range { get; set; }
    public bool ConvertToTRY { get; set; } = false;
}

public class GetAnalysisEntriesQueryHandler : IRequestHandler<GetAnalysisEntriesQuery, Result<AnalysisEntriesResponse>>
{
    private readonly IStatisticsRepository _statisticsRepository;

    public GetAnalysisEntriesQueryHandler(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }

    public async Task<Result<AnalysisEntriesResponse>> Handle(GetAnalysisEntriesQuery request, CancellationToken cancellationToken)
    {
        var result = await _statisticsRepository.GetAnalysisEntriesAsync(request);
        return Result.Success(result);
    }
}