using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using MediatR;

namespace BudgetFlow.Application.Currencies.Commands.SyncExchangeRates;
public class SyncExchangeRatesCommand : IRequest<Result<bool>>
{
    public class SyncExchangeRatesCommandHandler : IRequestHandler<SyncExchangeRatesCommand, Result<bool>>
    {
        private readonly IExchangeRateScraper _exchangeRateScraper;
        public SyncExchangeRatesCommandHandler(
            IExchangeRateScraper exchangeRateScraper)
        {
            _exchangeRateScraper = exchangeRateScraper;
        }

        public async Task<Result<bool>> Handle(SyncExchangeRatesCommand request, CancellationToken cancellationToken)
        {
            #region TCMB Scrapper
            var result = await _exchangeRateScraper.SyncExchangeRatesAsync();
            if (result.IsFailure)
                return Result.Failure<bool>(result.Error);
            #endregion

            return Result.Success(true);
        }
    }
}
