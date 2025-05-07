namespace BudgetFlow.Domain.Common;
public class BaseEntity:IAuditableEntity
{
    public int ID { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
