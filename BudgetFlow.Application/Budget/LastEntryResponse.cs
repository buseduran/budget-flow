using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Budget
{
    public class LastEntryResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public EntryType Type { get; set; }
        public string CategoryName { get; set; }
    }
}
