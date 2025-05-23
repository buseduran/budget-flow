namespace BudgetFlow.Application.Statistics.Responses;

public class AssetInvestInfoResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Unit { get; set; }
    public string Symbol { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalPrice { get; set; }
}