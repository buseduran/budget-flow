using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Budget
{
    public class GroupedEntry
    {
        public int CategoryID { get; set; }
        public CategoryDto Category { get; set; }
        public decimal Amount { get; set; }
        public string Color { get; set; }
    }
    public class GroupedEntriesResponse
    {
        public List<GroupedEntry> Incomes { get; set; } = new();
        public List<GroupedEntry> Expenses { get; set; } = new();
        public decimal? IncomeTrendPercentage { get; set; }
        public decimal? ExpenseTrendPercentage { get; set; }
    }
}
