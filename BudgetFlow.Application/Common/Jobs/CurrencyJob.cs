using BudgetFlow.Application.Common.Services.Abstract;
using Quartz;

namespace BudgetFlow.Application.Common.Jobs;
public class CurrencyJob : IJob
{
    private readonly IExchangeRateScraper exchangeRateScraper;
    public CurrencyJob(IExchangeRateScraper exchangeRateScraper)
    {
        this.exchangeRateScraper = exchangeRateScraper;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var result = await exchangeRateScraper.SyncExchangeRatesAsync();
        if (result.IsFailure)
        {
            throw new Exception(result.Error.ToString());
        }
    }
}
