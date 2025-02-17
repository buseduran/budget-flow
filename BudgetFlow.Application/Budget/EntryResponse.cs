using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Budget
{
    public class EntryResponse
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public EntryType Type { get; set; }
        public Category Category { get; set; }
    }
}
