using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Statistics.Responses;
using MediatR;

namespace BudgetFlow.Application.Statistics.Queries.GetLastEntries;

public record GetLastEntriesQuery : IRequest<Result<List<LastEntryResponse>>>
{
    public int WalletID { get; set; }
}

public class GetLastEntriesQueryHandler : IRequestHandler<GetLastEntriesQuery, Result<List<LastEntryResponse>>>
{
    private readonly IStatisticsRepository _statisticsRepository;

    public GetLastEntriesQueryHandler(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }

    public async Task<Result<List<LastEntryResponse>>> Handle(GetLastEntriesQuery request, CancellationToken cancellationToken)
    {
        var result = await _statisticsRepository.GetLastEntriesAsync(request);
        return Result.Success(result);
    }
}