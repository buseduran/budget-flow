using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class Portfolio : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Investment> Investments { get; set; } = new List<Investment>();
    }
}
