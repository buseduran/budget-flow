using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Queries.GetAnalysisEntries;
public class GetAnalysisEntriesQuery : IRequest<Result<AnalysisEntriesResponse>>
{
    public string Range { get; set; }
    public int WalletID { get; set; }
    public bool ConvertToTRY { get; set; } = false;
    public class GetAnalysisEntriesQueryHandler : IRequestHandler<GetAnalysisEntriesQuery, Result<AnalysisEntriesResponse>>
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IUserWalletRepository userWalletRepository;
        private readonly ICurrencyRateRepository currencyRateRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetAnalysisEntriesQueryHandler(
            IBudgetRepository budgetRepository,
            IHttpContextAccessor httpContextAccessor,
            IUserWalletRepository userWalletRepository,
            ICurrencyRateRepository currencyRateRepository)
        {
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.userWalletRepository = userWalletRepository;
            this.currencyRateRepository = currencyRateRepository;
        }
        public async Task<Result<AnalysisEntriesResponse>> Handle(GetAnalysisEntriesQuery request, CancellationToken cancellationToken)
        {
            int userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            var userWallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);

            decimal exchangeRateToTRY = 1m;
            if (request.ConvertToTRY && userWallet.Wallet.Currency != CurrencyType.TRY)
            {
                var rate = await currencyRateRepository.GetCurrencyRateByType(userWallet.Wallet.Currency);
                exchangeRateToTRY = rate.ForexSelling;
            }

            var entries = await budgetRepository.GetAnalysisEntriesAsync(
                userID, 
                request.Range, 
                userWallet.Wallet.Currency, 
                request.WalletID,
                exchangeRateToTRY,
                request.ConvertToTRY);

            return entries != null
                ? Result.Success(entries)
                : Result.Failure<AnalysisEntriesResponse>(EntryErrors.AnalysisEntriesRetrievalFailed);
        }
    }
}
