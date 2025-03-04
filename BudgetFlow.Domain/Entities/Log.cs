using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities;
public class Log : BaseEntity
{
    public string Action { get; set; }
    public DateTime TimeStamp { get; set; }

    public int UserID { get; set; }
    public User User { get; set; }
}
