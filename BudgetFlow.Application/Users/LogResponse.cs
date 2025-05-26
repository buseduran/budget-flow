using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Users;
public class LogResponse
{
    public int ID { get; set; }
    public string TableName { get; set; }
    public LogType Action { get; set; }
    public string PrimaryKey { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
    public DateTime Timestamp { get; set; }
}
