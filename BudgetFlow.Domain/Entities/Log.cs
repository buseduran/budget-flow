namespace BudgetFlow.Domain.Entities;

public class AuditLog
{
    public int ID { get; set; }
    public string TableName { get; set; }
    public string Action { get; set; } // Insert, Update, Delete
    public string PrimaryKey { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
    public int UserID { get; set; } 
    public DateTime Timestamp { get; set; }
}
