using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface ICurrencyRateRepository
{
    Task DeleteRatesForDateAsync(DateTime date, bool saveChanges = true);
    Task AddRatesAsync(IEnumerable<CurrencyRate> rates, bool saveChanges = true);
    Task<CurrencyRate> GetCurrencyRateByType(CurrencyType currency);
}
