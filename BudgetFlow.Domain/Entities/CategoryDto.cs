using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities
{
    public class CategoryDto : BaseEntity
    {
        public string Name { get; set; }
        public string Color { get; set; }

        public int UserID { get; set; }
        public UserDto User { get; set; }
    }
}
