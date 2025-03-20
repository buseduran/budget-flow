using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public List<Log> Logs { get; set; }
        public List<Portfolio> Portfolios { get; set; }
        public Wallet Wallet { get; set; }
    }
}
