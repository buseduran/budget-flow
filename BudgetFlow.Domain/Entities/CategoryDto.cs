using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class CategoryDto : BaseEntity
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
