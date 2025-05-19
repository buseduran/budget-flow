using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities;
public class Invitation:BaseEntity
{
    public string Email { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}
