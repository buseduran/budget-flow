using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities;
public class Asset : BaseEntity
{
    public string Name { get; set; }
    public AssetType AssetType { get; set; }
    public decimal BuyPrice { get; set; }
    public decimal SellPrice { get; set; }
    public string Description { get; set; }
    public string Symbol { get; set; }
    public string Code { get; set; }
    public string Unit { get; set; }
    public ICollection<Investment> Investments { get; set; }
}
