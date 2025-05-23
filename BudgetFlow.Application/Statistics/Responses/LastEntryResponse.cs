using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Statistics.Responses;

public class LastEntryResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal AmountInTRY { get; set; }
    public decimal ExchangeRate { get; set; }
    public CurrencyType Currency { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}