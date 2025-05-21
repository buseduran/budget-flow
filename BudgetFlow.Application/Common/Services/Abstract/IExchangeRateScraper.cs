using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Services.Abstract;
public interface IExchangeRateScraper
{
    Task<IEnumerable<CurrencyRate>> GetExchangeRatesAsync();
    Task<Result<bool>> SyncExchangeRatesAsync();
}
