using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;
public class CurrencyRateRepository : ICurrencyRateRepository
{
    private readonly BudgetContext context;
    public CurrencyRateRepository(BudgetContext context)
    {
        this.context = context;
    }
    public async Task DeleteRatesForDateAsync(DateTime date, bool saveChanges = true)
    {
        var startDate = date.Date;
        var endDate = startDate.AddDays(1);

        var ratesToDelete = await context.CurrencyRates
            .Where(r => r.RetrievedAt >= startDate && r.RetrievedAt < endDate)
            .ToListAsync();
        context.CurrencyRates.RemoveRange(ratesToDelete);
        if (saveChanges)
            await context.SaveChangesAsync();
    }
    public async Task AddRatesAsync(IEnumerable<CurrencyRate> rates, bool saveChanges = true)
    {
        await context.CurrencyRates.AddRangeAsync(rates);
        if (saveChanges)
            await context.SaveChangesAsync();
    }

    public async Task<CurrencyRate> GetCurrencyRateByType(CurrencyType currency)
    {
        var currencyRate = await context.CurrencyRates
            .FirstOrDefaultAsync(c => c.CurrencyType == currency);
        return currencyRate;
    }

    public async Task UpdateRateAsync(CurrencyRate rate, bool saveChanges = true)
    {
        context.CurrencyRates.Update(rate);
        if (saveChanges)
            await context.SaveChangesAsync();
    }
}
