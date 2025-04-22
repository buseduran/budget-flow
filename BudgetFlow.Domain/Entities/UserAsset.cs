using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities;
public class UserAsset : BaseEntity
{
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public int AssetId { get; set; }
    public virtual Asset Asset { get; set; }
}
