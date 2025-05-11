namespace BudgetFlow.Application.Auth;
public class LogResponse
{
    public int ID { get; set; }
    public string TableName { get; set; }
    public string Action { get; set; }
    public string PrimaryKey { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
    public DateTime Timestamp { get; set; }
}
