namespace BudgetFlow.Application.Common.Services.Abstract;
public interface IExchangeRateScraper
{
    Task<IEnumerable<decimal>> GetExchangeRatesAsync();
}

