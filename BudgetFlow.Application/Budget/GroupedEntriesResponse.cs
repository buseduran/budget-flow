using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Budget;
public class AnalysisEntry
{
    public int CategoryID { get; set; }
    public Category Category { get; set; }
    public string CategoryName { get; set; }
    public decimal Amount { get; set; }
    public string Color { get; set; }
}
public class AnalysisEntriesResponse
{
    public List<AnalysisEntry> Incomes { get; set; } = new();
    public List<AnalysisEntry> Expenses { get; set; } = new();
    public decimal? IncomeTrendPercentage { get; set; }
    public decimal? ExpenseTrendPercentage { get; set; }
}
