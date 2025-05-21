using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using MediatR;

namespace BudgetFlow.Application.Currencies.Commands.SyncExchangeRates;
public class SyncExchangeRatesCommand : IRequest<Result<bool>>
{
    public class SyncExchangeRatesCommandHandler : IRequestHandler<SyncExchangeRatesCommand, Result<bool>>
    {
        private readonly ICurrencyRateRepository currencyRateRepository;
        private readonly IExchangeRateScraper exchangeRateScraper;
        public SyncExchangeRatesCommandHandler(
            ICurrencyRateRepository currencyRateRepository,
            IExchangeRateScraper exchangeRateScraper)
        {
            this.currencyRateRepository = currencyRateRepository;
            this.exchangeRateScraper = exchangeRateScraper;
        }

        public async Task<Result<bool>> Handle(SyncExchangeRatesCommand request, CancellationToken cancellationToken)
        {
            #region TCMB Scrapper
            var result = await exchangeRateScraper.SyncExchangeRatesAsync();
            if (result.IsFailure)
                return Result.Failure<bool>(result.Error);
            #endregion

            return Result.Success(true);
        }
    }
}
