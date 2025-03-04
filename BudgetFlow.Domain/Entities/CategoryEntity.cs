using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities
{
    public class CategoryEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Color { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }
    }
}
