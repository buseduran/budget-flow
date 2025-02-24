using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Budget
{
    public class GroupedEntry
    {
        public Category Category { get; set; }
        public decimal Amount { get; set; }
    }
    public class GroupedEntriesResponse
    {
        public List<GroupedEntry> Incomes { get; set; } = new();
        public List<GroupedEntry> Expenses { get; set; } = new();
        public decimal? IncomeTrendPercentage { get; set; }
        public decimal? ExpenseTrendPercentage { get; set; }
    }
}
