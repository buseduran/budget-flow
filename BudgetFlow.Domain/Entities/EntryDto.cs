using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities
{
    public class EntryDto : BaseEntity
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public EntryType Type { get; set; }
        public CategoryType CategoryType { get; set; }
        public DateTime Date { get; set; }

        public int UserID { get; set; }
        public UserDto User { get; set; }
    }
}
