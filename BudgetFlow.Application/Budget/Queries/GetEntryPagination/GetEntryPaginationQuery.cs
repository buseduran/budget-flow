using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Queries.GetEntryPagination;
public class GetEntryPaginationQuery : IRequest<Result<PaginatedList<EntryResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int WalletID { get; set; }
    public class GetEntryPaginationQueryHandler : IRequestHandler<GetEntryPaginationQuery, Result<PaginatedList<EntryResponse>>>
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IUserWalletRepository userWalletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetEntryPaginationQueryHandler(
            IBudgetRepository budgetRepository,
            IHttpContextAccessor httpContextAccessor,
            IUserWalletRepository userWalletRepository)
        {
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.userWalletRepository = userWalletRepository;
        }

        public async Task<Result<PaginatedList<EntryResponse>>> Handle(GetEntryPaginationQuery request, CancellationToken cancellationToken)
        {
            int userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            var userWallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);

            var result = await budgetRepository.GetPaginatedAsync(request.Page, request.PageSize, userID, userWallet.Wallet.Currency, request.WalletID);
            if (result == null)
                return Result.Failure<PaginatedList<EntryResponse>>(EntryErrors.EntryNotFound);

            return Result.Success(result);
        }
    }
}
