using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using MediatR;

namespace BudgetFlow.Application.Budget.Queries.GetEntryPagination
{
    public class GetEntryPaginationQuery : IRequest<PaginatedList<EntryResponse>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public class GetEntryPaginationQueryHandler : IRequestHandler<GetEntryPaginationQuery, PaginatedList<EntryResponse>>
        {
            private readonly IBudgetRepository _budgetRepository;
            public GetEntryPaginationQueryHandler(IBudgetRepository budgetRepository)
            {
                _budgetRepository = budgetRepository;
            }

            public async Task<PaginatedList<EntryResponse>> Handle(GetEntryPaginationQuery request, CancellationToken cancellationToken)
            {
                var result = await _budgetRepository.GetPaginatedAsync(request.Page, request.PageSize);

                return result;
            }
        }
    }
}