using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities
{
    public class EntryEntity : BaseEntity
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public EntryType Type { get; set; }
        public DateTime Date { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public int CategoryID { get; set; }
        public CategoryEntity Category { get; set; }
    }
}
