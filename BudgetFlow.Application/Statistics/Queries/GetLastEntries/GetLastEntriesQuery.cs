using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Statistics.Queries.GetLastEntries;

public record GetLastEntriesQuery : IRequest<Result<List<LastEntryResponse>>>
{
    public int WalletID { get; set; }
}

public class GetLastEntriesQueryHandler : IRequestHandler<GetLastEntriesQuery, Result<List<LastEntryResponse>>>
{
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IUserWalletRepository _userWalletRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetLastEntriesQueryHandler(IStatisticsRepository statisticsRepository, IUserWalletRepository userWalletRepository, IHttpContextAccessor httpContextAccessor)
    {
        _statisticsRepository = statisticsRepository;
        _userWalletRepository = userWalletRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<List<LastEntryResponse>>> Handle(GetLastEntriesQuery request, CancellationToken cancellationToken)
    {
        int userID = new GetCurrentUser(_httpContextAccessor).GetCurrentUserID();
        var userWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
        if (userWallet == null)
            return Result.Failure<List<LastEntryResponse>>(UserWalletErrors.UserWalletNotFound);

        var result = await _statisticsRepository.GetLastEntriesAsync(request.WalletID);
        return Result.Success(result);
    }
}