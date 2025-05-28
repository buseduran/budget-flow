using BudgetFlow.Application.Common.Services.Abstract;
using Quartz;

namespace BudgetFlow.Application.Common.Jobs;
public class CurrencyJob : IJob
{
    private readonly IExchangeRateScraper _exchangeRateScraper;
    public CurrencyJob(IExchangeRateScraper exchangeRateScraper)
    {
        _exchangeRateScraper = exchangeRateScraper;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await ExecuteAsync();
    }

    public async Task ExecuteAsync()
    {
        var result = await _exchangeRateScraper.SyncExchangeRatesAsync();
        if (result.IsFailure)
        {
            throw new Exception(result.Error.ToString());
        }
    }
}
