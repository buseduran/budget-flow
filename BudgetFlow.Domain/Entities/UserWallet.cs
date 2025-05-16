using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities;
public class UserWallet : BaseEntity
{
    public int WalletID { get; set; }
    public Wallet Wallet { get; set; }
    public int UserID { get; set; }
    public User User { get; set; }
    public WalletRole Role { get; set; } = WalletRole.Viewer;
}
