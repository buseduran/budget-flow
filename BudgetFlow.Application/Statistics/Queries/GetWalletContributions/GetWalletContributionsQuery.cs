using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Statistics.Responses;
using MediatR;

namespace BudgetFlow.Application.Statistics.Queries.GetWalletContributions;

public record GetWalletContributionsQuery(int WalletId, bool ConvertToTRY = false) : IRequest<Result<List<WalletContributionResponse>>>;

public class GetWalletContributionsQueryHandler : IRequestHandler<GetWalletContributionsQuery, Result<List<WalletContributionResponse>>>
{
    private readonly IStatisticsRepository _statisticsRepository;

    public GetWalletContributionsQueryHandler(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }

    public async Task<Result<List<WalletContributionResponse>>> Handle(GetWalletContributionsQuery request, CancellationToken cancellationToken)
    {
        var contributions = await _statisticsRepository.GetWalletContributionsAsync(request.WalletId, request.ConvertToTRY);
        return Result.Success(contributions);
    }
}