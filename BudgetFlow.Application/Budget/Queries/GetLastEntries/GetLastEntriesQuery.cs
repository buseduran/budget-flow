using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Queries.GetLastEntries;
public class GetLastEntriesQuery : IRequest<Result<List<LastEntryResponse>>>
{
    public int WalletID { get; set; }
    public class GetLastFiveEntriesQueryHandler : IRequestHandler<GetLastEntriesQuery, Result<List<LastEntryResponse>>>
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetLastFiveEntriesQueryHandler(IBudgetRepository budgetRepository, IHttpContextAccessor httpContextAccessor, IWalletRepository walletRepository)
        {
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.walletRepository = walletRepository;
        }
        public async Task<Result<List<LastEntryResponse>>> Handle(GetLastEntriesQuery request, CancellationToken cancellationToken)
        {
            GetCurrentUser getCurrentUser = new(httpContextAccessor);
            int userID = getCurrentUser.GetCurrentUserID();

            var currency = await walletRepository.GetUserCurrencyAsync(userID);

            var entries = await budgetRepository.GetLastFiveEntriesAsync(userID, currency, request.WalletID);
            return entries != null
                ? Result.Success(entries)
                : Result.Failure<List<LastEntryResponse>>(EntryErrors.LatestEntriesRetrievalFailed);
        }
    }
}
