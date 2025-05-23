using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Statistics.Responses;

public class AssetInvestResponse
{
    public int ID { get; set; }
    public decimal UnitAmount { get; set; }
    public decimal CurrencyAmount { get; set; }
    public decimal AmountInTRY { get; set; }
    public decimal ExchangeRate { get; set; }
    public string Description { get; set; }
    public InvestmentType Type { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}