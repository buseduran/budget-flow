using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities;
public class Wallet : BaseEntity
{
    public decimal Balance { get; set; }
    public decimal BalanceInTRY { get; set; }
    public CurrencyType Currency { get; set; }
    public ICollection<User> User { get; set; }
}
