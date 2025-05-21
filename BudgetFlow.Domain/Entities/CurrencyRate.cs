using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities;
public class CurrencyRate
{
    public int ID { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public decimal ForexBuying { get; set; }
    public decimal ForexSelling { get; set; }
    public DateTime RetrievedAt { get; set; }
}
