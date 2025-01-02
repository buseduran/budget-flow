using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities
{
    public class EntryDto : BaseEntity
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public EntryType Type { get; set; }
        public Category Category { get; set; }
        public DateTime Date { get; set; }
    }
}
