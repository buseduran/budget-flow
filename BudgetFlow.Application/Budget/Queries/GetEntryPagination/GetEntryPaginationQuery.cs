using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Budget.Queries.GetEntryPagination;
public class GetEntryPaginationQuery : IRequest<Result<PaginatedList<EntryResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int WalletID { get; set; }
    public int? UserID { get; set; }
    public class GetEntryPaginationQueryHandler : IRequestHandler<GetEntryPaginationQuery, Result<PaginatedList<EntryResponse>>>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetEntryPaginationQueryHandler(
            IBudgetRepository budgetRepository,
            ICurrentUserService currentUserService,
            IUserWalletRepository userWalletRepository)
        {
            _budgetRepository = budgetRepository;
            _currentUserService = currentUserService;
            _userWalletRepository = userWalletRepository;
        }

        public async Task<Result<PaginatedList<EntryResponse>>> Handle(GetEntryPaginationQuery request, CancellationToken cancellationToken)
        {
            int userID = _currentUserService.GetCurrentUserID();

            var userWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
            if (userWallet == null)
                return Result.Failure<PaginatedList<EntryResponse>>(UserWalletErrors.UserWalletNotFound);

            var result = await _budgetRepository.GetPaginatedAsync(request.Page, request.PageSize, request.WalletID, request.UserID);
            if (result == null)
                return Result.Failure<PaginatedList<EntryResponse>>(EntryErrors.EntryNotFound);

            return Result.Success(result);
        }
    }
}
