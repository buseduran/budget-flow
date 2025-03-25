using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities;
public class Wallet : BaseEntity
{
    public decimal Balance { get; set; }
    public CurrencyType Currency { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}

