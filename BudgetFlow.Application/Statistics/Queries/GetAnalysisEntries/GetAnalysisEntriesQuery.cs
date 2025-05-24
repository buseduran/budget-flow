using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

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
    private readonly IUserWalletRepository _userWalletRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public GetAnalysisEntriesQueryHandler(
        IStatisticsRepository statisticsRepository,
        IUserWalletRepository userWalletRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _statisticsRepository = statisticsRepository;
        _userWalletRepository = userWalletRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<AnalysisEntriesResponse>> Handle(GetAnalysisEntriesQuery request, CancellationToken cancellationToken)
    {
        int userID = new GetCurrentUser(_httpContextAccessor).GetCurrentUserID();
        var userWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
        if (userWallet == null)
            return Result.Failure<AnalysisEntriesResponse>(UserWalletErrors.UserWalletNotFound);

        var result = await _statisticsRepository.GetAnalysisEntriesAsync(request.WalletID, request.Range, request.ConvertToTRY);
        return Result.Success(result);
    }
}