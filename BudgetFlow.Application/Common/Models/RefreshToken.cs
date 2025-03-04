using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Models
{
    public class RefreshToken
    {
        public Guid ID { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }
    }
}
