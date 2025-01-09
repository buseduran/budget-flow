using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class UserDto : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public IEnumerable<BudgetDto> Budgets { get; set; }
        public IEnumerable<LogDto> Logs { get; set; }
    }
}
