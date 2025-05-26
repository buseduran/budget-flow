using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Statistics.Queries.GetLastEntries;

public record GetLastEntriesQuery : IRequest<Result<List<LastEntryResponse>>>
{
    public int WalletID { get; set; }
}

public class GetLastEntriesQueryHandler : IRequestHandler<GetLastEntriesQuery, Result<List<LastEntryResponse>>>
{
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IUserWalletRepository _userWalletRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetLastEntriesQueryHandler(IStatisticsRepository statisticsRepository, IUserWalletRepository userWalletRepository, ICurrentUserService currentUserService)
    {
        _statisticsRepository = statisticsRepository;
        _userWalletRepository = userWalletRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<LastEntryResponse>>> Handle(GetLastEntriesQuery request, CancellationToken cancellationToken)
    {
        int userID = _currentUserService.GetCurrentUserID();
        var userWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
        if (userWallet == null)
            return Result.Failure<List<LastEntryResponse>>(UserWalletErrors.UserWalletNotFound);

        var result = await _statisticsRepository.GetLastEntriesAsync(request.WalletID);
        return Result.Success(result);
    }
}