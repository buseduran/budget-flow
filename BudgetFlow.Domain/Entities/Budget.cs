using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class Budget : BaseEntity
    {
        public int TargetAmount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }
    }
}
