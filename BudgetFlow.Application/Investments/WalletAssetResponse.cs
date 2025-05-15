namespace BudgetFlow.Application.Investments;
public class WalletAssetResponse
{
    public int ID { get; set; }
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }
    public int WalletId { get; set; }
    public int AssetId { get; set; }
}
