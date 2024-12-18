using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities;
public class BudgetDto : BaseEntity
{
    public int TargetAmount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }

    public int UserID { get; set; }
    public UserDto User { get; set; }
}
