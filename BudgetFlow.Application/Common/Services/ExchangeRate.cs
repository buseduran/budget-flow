namespace BudgetFlow.Application.Common.Services;
public class ExchangeRate
{
    public string Code { get; set; }
    public string Name { get; set; }
    public int Unit { get; set; }
    public decimal BuyingPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public DateTime UpdatedAt { get; set; }
}
