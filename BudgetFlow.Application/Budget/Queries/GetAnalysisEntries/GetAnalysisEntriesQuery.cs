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
    public GetAnalysisEntriesQuery(string Range, int WalletID)
    {
        this.Range = Range;
        this.WalletID = WalletID;
    }
    public class GetAnalysisEntriesQueryHandler : IRequestHandler<GetAnalysisEntriesQuery, Result<AnalysisEntriesResponse>>
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetAnalysisEntriesQueryHandler(IBudgetRepository budgetRepository, IHttpContextAccessor httpContextAccessor, IWalletRepository walletRepository)
        {
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.walletRepository = walletRepository;
        }
        public async Task<Result<AnalysisEntriesResponse>> Handle(GetAnalysisEntriesQuery request, CancellationToken cancellationToken)
        {
            GetCurrentUser getCurrentUser = new(httpContextAccessor);
            int userID = getCurrentUser.GetCurrentUserID();

            var currency = await walletRepository.GetUserCurrencyAsync(userID);

            var entries = await budgetRepository.GetAnalysisEntriesAsync(userID, request.Range, currency, request.WalletID);
            return entries != null
                ? Result.Success(entries)
                : Result.Failure<AnalysisEntriesResponse>(EntryErrors.AnalysisEntriesRetrievalFailed);
        }
    }
}
