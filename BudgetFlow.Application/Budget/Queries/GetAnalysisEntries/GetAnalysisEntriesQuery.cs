using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Queries.GetAnalysisEntries;
public class GetAnalysisEntriesQuery : IRequest<Result<AnalysisEntriesResponse>>
{
    public string Range { get; set; }
    public int WalletID { get; set; }
    public class GetAnalysisEntriesQueryHandler : IRequestHandler<GetAnalysisEntriesQuery, Result<AnalysisEntriesResponse>>
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IUserWalletRepository userWalletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetAnalysisEntriesQueryHandler(
            IBudgetRepository budgetRepository,
            IHttpContextAccessor httpContextAccessor,
            IUserWalletRepository userWalletRepository)
        {
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.userWalletRepository = userWalletRepository;
        }
        public async Task<Result<AnalysisEntriesResponse>> Handle(GetAnalysisEntriesQuery request, CancellationToken cancellationToken)
        {
            int userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            var userWallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);

            var entries = await budgetRepository.GetAnalysisEntriesAsync(userID, request.Range, userWallet.Wallet.Currency, request.WalletID);
            return entries != null
                ? Result.Success(entries)
                : Result.Failure<AnalysisEntriesResponse>(EntryErrors.AnalysisEntriesRetrievalFailed);
        }
    }
}
