using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Scrapers.Abstract;
public interface IExchangeRateScraper
{
    Task<IEnumerable<CurrencyRate>> GetExchangeRatesAsync();
    Task<Result<bool>> SyncExchangeRatesAsync();
}
