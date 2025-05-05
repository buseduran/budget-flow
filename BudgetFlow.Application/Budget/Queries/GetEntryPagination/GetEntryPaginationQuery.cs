using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Queries.GetEntryPagination;
public class GetEntryPaginationQuery : IRequest<Result<PaginatedList<EntryResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public class GetEntryPaginationQueryHandler : IRequestHandler<GetEntryPaginationQuery, Result<PaginatedList<EntryResponse>>>
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetEntryPaginationQueryHandler(IBudgetRepository budgetRepository, IHttpContextAccessor httpContextAccessor, IWalletRepository walletRepository)
        {
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.walletRepository = walletRepository;
        }

        public async Task<Result<PaginatedList<EntryResponse>>> Handle(GetEntryPaginationQuery request, CancellationToken cancellationToken)
        {
            GetCurrentUser getCurrentUser = new(httpContextAccessor);
            int userID = getCurrentUser.GetCurrentUserID();

            var currency = await walletRepository.GetUserCurrencyAsync(userID);

            var result = await budgetRepository.GetPaginatedAsync(request.Page, request.PageSize, userID, currency);
            if (result == null)
                return Result.Failure<PaginatedList<EntryResponse>>(EntryErrors.EntryNotFound);

            return Result.Success(result);
        }
    }
}
