using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities;
public class Category : BaseEntity
{
    public string Name { get; set; }
    public string Color { get; set; }

    public int UserID { get; set; }
    public User User { get; set; }

}
