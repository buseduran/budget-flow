using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Queries.GetEntryPagination
{
    public class GetEntryPaginationQuery : IRequest<PaginatedList<EntryResponse>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public class GetEntryPaginationQueryHandler : IRequestHandler<GetEntryPaginationQuery, PaginatedList<EntryResponse>>
        {
            private readonly IBudgetRepository _budgetRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public GetEntryPaginationQueryHandler(IBudgetRepository budgetRepository, IHttpContextAccessor httpContextAccessor)
            {
                _budgetRepository = budgetRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<PaginatedList<EntryResponse>> Handle(GetEntryPaginationQuery request, CancellationToken cancellationToken)
            {
                var context = _httpContextAccessor.HttpContext;
                GetCurrentUser getCurrentUser = new(_httpContextAccessor);
                int userID = getCurrentUser.GetCurrentUserID();



                var result = await _budgetRepository.GetPaginatedAsync(request.Page, request.PageSize,userID);

                return result;
            }
        }
    }
}