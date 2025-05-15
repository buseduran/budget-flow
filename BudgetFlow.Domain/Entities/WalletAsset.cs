using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities;
public class WalletAsset : BaseEntity
{
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }

    public int WalletId { get; set; }
    public Wallet Wallet { get; set; }

    public int AssetId { get; set; }
    public Asset Asset { get; set; }
}
