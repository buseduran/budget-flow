using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities;

public class TransactionDto : BaseEntity
{
    public int Amount { get; set; }
    public Category Category { get; set; }
    public string Description { get; set; }
    public EntryType Type { get; set; }
    public DateTime TransactionDate { get; set; }

    public int UserID { get; set; }
    public UserDto User { get; set; }
}
