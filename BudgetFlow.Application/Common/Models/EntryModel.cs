using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Models
{
    public class EntryModel
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public EntryType Type { get; set; }
        public Category Category { get; set; }
    }
}
