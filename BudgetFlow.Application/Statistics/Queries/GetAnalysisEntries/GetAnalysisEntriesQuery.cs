using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Statistics.Queries.GetAnalysisEntries;

public record GetAnalysisEntriesQuery : IRequest<Result<AnalysisEntriesResponse>>
{
    public int WalletID { get; set; }
    public string Range { get; set; }
    public bool ConvertToTRY { get; set; } = false;

    public class GetAnalysisEntriesQueryHandler : IRequestHandler<GetAnalysisEntriesQuery, Result<AnalysisEntriesResponse>>
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrencyRateRepository currencyRateRepository;
        public GetAnalysisEntriesQueryHandler(
            IStatisticsRepository statisticsRepository,
            IUserWalletRepository userWalletRepository,
            IHttpContextAccessor httpContextAccessor,
            ICurrencyRateRepository currencyRateRepository)
        {
            _statisticsRepository = statisticsRepository;
            _userWalletRepository = userWalletRepository;
            _httpContextAccessor = httpContextAccessor;
            this.currencyRateRepository = currencyRateRepository;
        }

        public async Task<Result<AnalysisEntriesResponse>> Handle(GetAnalysisEntriesQuery request, CancellationToken cancellationToken)
        {
            int userID = new GetCurrentUser(_httpContextAccessor).GetCurrentUserID();
            var userWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
            if (userWallet == null)
                return Result.Failure<AnalysisEntriesResponse>(UserWalletErrors.UserWalletNotFound);

            decimal exchangeRateToTRY = 1m;
            if (request.ConvertToTRY && userWallet.Wallet.Currency != CurrencyType.TRY)
            {
                var rate = await currencyRateRepository.GetCurrencyRateByType(userWallet.Wallet.Currency);
                exchangeRateToTRY = rate.ForexSelling;
            }

            var entries = await _statisticsRepository.GetAnalysisEntriesAsync(
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