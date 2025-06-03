using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities;
public class Wallet : BaseEntity
{
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public ICollection<Category> Categories { get; set; }
}
