using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Queries.GetLastEntries;
public class GetLastEntriesQuery : IRequest<Result<List<LastEntryResponse>>>
{
    public class GetLastFiveEntriesQueryHandler : IRequestHandler<GetLastEntriesQuery, Result<List<LastEntryResponse>>>
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetLastFiveEntriesQueryHandler(IBudgetRepository budgetRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<List<LastEntryResponse>>> Handle(GetLastEntriesQuery request, CancellationToken cancellationToken)
        {
            GetCurrentUser getCurrentUser = new(httpContextAccessor);
            int userID = getCurrentUser.GetCurrentUserID();
            var entries = await budgetRepository.GetLastFiveEntriesAsync(userID);
            return entries != null
                ? Result.Success(entries)
                : Result.Failure<List<LastEntryResponse>>("Failed to get last entries");
        }
    }
}
