using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities;
public class Portfolio : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int WalletID { get; set; }
    public Wallet Wallet { get; set; }
    public ICollection<Investment> Investments { get; set; }
}
