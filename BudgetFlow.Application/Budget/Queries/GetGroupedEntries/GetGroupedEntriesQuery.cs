using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Queries.GetGroupedEntries;
public class GetGroupedEntriesQuery : IRequest<Result<GroupedEntriesResponse>>
{
    public string Range { get; set; }
    public GetGroupedEntriesQuery(string Range)
    {
        this.Range = Range;
    }
    public class GetGroupedEntriesQueryHandler : IRequestHandler<GetGroupedEntriesQuery, Result<GroupedEntriesResponse>>
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetGroupedEntriesQueryHandler(IBudgetRepository budgetRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<GroupedEntriesResponse>> Handle(GetGroupedEntriesQuery request, CancellationToken cancellationToken)
        {
            GetCurrentUser getCurrentUser = new(httpContextAccessor);
            int userID = getCurrentUser.GetCurrentUserID();

            var entries = await budgetRepository.GetGroupedEntriesAsync(userID, request.Range);
            return entries != null
                ? Result.Success(entries)
                : Result.Failure<GroupedEntriesResponse>("Failed to retrieve grouped entries.");
        }
    }
}
