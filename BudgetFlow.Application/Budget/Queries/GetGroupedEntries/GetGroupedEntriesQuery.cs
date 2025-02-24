using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Queries.GetGroupedEntries
{
    public class GetGroupedEntriesQuery : IRequest<GroupedEntriesResponse>
    {
        public string Range { get; set; }
        public GetGroupedEntriesQuery(string Range)
        {
            this.Range = Range;
        }
        public class GetGroupedEntriesQueryHandler : IRequestHandler<GetGroupedEntriesQuery, GroupedEntriesResponse>
        {
            private readonly IBudgetRepository _budgetRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public GetGroupedEntriesQueryHandler(IBudgetRepository budgetRepository, IHttpContextAccessor httpContextAccessor)
            {
                _budgetRepository = budgetRepository;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<GroupedEntriesResponse> Handle(GetGroupedEntriesQuery request, CancellationToken cancellationToken)
            {
                GetCurrentUser getCurrentUser = new(_httpContextAccessor);
                int userID = getCurrentUser.GetCurrentUserID();

                return await _budgetRepository.GetGroupedEntriesAsync(userID, request.Range);
            }
        }
    }
}
