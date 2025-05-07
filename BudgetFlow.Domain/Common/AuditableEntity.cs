namespace BudgetFlow.Domain.Common;

public abstract class AuditableEntity : IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UpdatedBy { get; set; }
}
