using BudgetFlow.Application.Categories;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Budget;
public class AnalysisEntry
{
    public CategoryResponse Category { get; set; }
    public decimal Amount { get; set; }
    public CurrencyType Currency { get; set; }
}
public class AnalysisEntriesResponse
{
    public List<AnalysisEntry> Incomes { get; set; } = new();
    public List<AnalysisEntry> Expenses { get; set; } = new();
    public decimal? IncomeTrendPercentage { get; set; }
    public decimal? ExpenseTrendPercentage { get; set; }
}
