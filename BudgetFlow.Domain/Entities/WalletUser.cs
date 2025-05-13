using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities;
public class WalletUser : BaseEntity
{
    public int WalletID { get; set; }
    public Wallet Wallet { get; set; }
    public int UserID { get; set; }
    public User User { get; set; }
}
