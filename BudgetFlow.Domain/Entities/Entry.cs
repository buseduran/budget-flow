using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities;
public class Entry : BaseEntity
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public decimal AmountInTRY { get; set; }
    public decimal ExchangeRate { get; set; }
    public CurrencyType Currency { get; set; }
    public DateTime Date { get; set; }

    public int UserID { get; set; }
    public User User { get; set; }

    public int WalletID { get; set; }
    public Wallet Wallet { get; set; }

    public int CategoryID { get; set; }
    public Category Category { get; set; }
}
