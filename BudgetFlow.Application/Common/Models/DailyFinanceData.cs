namespace BudgetFlow.Application.Common.Models;

public class DailyFinanceData
{
    public DateTime Date { get; set; }
    public decimal TotalSpent { get; set; }
    public List<CategorySpending> Categories { get; set; } = new();
    public decimal YesterdayTotal { get; set; }
}

public class CategorySpending
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
}
