using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Currencies;
public class CurrencyResponse
{
    public CurrencyType CurrencyType { get; set; }
    public decimal ForexBuying { get; set; }
    public decimal ForexSelling { get; set; }
    public DateTime RetrievedAt { get; set; }
}