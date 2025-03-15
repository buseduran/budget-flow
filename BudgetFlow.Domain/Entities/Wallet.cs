using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class Wallet : BaseEntity
    {
        public decimal Balance { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
