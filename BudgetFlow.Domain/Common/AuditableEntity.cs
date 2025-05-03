
namespace BudgetFlow.Domain.Common;

public class AuditableEntity : IAuditableEntity
{
    DateTime IAuditableEntity.CreatedAt { get; set; }
    string IAuditableEntity.CreatedBy { get; set; }
    DateTime IAuditableEntity.UpdatedAt { get; set; }
    string IAuditableEntity.UpdatedBy { get; set; }
}
