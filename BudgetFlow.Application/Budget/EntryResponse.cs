using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Budget
{
    public class EntryResponse
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public EntryType Type { get; set; }
        public CategoryType Category { get; set; }
    }
}
