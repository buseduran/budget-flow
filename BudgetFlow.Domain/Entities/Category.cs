using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities;
public class Category : BaseEntity
{
    public string Name { get; set; }
    public string Color { get; set; }
    public EntryType Type { get; set; }

    public int WalletID { get; set; }
    public Wallet Wallet { get; set; }

    public ICollection<Entry> Entries { get; set; }
}
