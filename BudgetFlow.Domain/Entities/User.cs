using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities;
public class User : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;

    public List<Portfolio> Portfolios { get; set; }
    public List<UserWallet> UserWallets { get; set; }
    public List<Entry> Entries { get; set; }
}
