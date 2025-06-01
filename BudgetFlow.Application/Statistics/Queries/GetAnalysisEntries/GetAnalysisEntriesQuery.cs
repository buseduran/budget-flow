using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Statistics.Queries.GetAnalysisEntries;

public record GetAnalysisEntriesQuery : IRequest<Result<AnalysisEntriesResponse>>
{
    public int WalletID { get; set; }
    public string Range { get; set; }
    public bool ConvertToTRY { get; set; } = false;

    public class GetAnalysisEntriesQueryHandler : IRequestHandler<GetAnalysisEntriesQuery, Result<AnalysisEntriesResponse>>
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetAnalysisEntriesQueryHandler(
            IStatisticsRepository statisticsRepository,
            IUserWalletRepository userWalletRepository,
            ICurrentUserService currentUserService)
        {
            _statisticsRepository = statisticsRepository;
            _userWalletRepository = userWalletRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<AnalysisEntriesResponse>> Handle(GetAnalysisEntriesQuery request, CancellationToken cancellationToken)
        {
            int userID = _currentUserService.GetCurrentUserID();
            var userWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
            if (userWallet == null)
                return Result.Failure<AnalysisEntriesResponse>(UserWalletErrors.UserWalletNotFound);


            var entries = await _statisticsRepository.GetAnalysisEntriesAsync(
                userID,
                request.Range,
                request.WalletID);

            return entries != null
               ? Result.Success(entries)
               : Result.Failure<AnalysisEntriesResponse>(EntryErrors.AnalysisEntriesRetrievalFailed);
        }
    }
}