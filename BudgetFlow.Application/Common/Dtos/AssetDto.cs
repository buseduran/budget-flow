using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Dtos;
public class AssetDto
{
    public string Name { get; set; }
    public AssetType AssetType { get; set; }
    public decimal BuyPrice { get; set; }
    public decimal SellPrice { get; set; }
    public string Description { get; set; }
    public string Code { get; set; } //XAU XPT
    public string Unit { get; set; } //gr vs
}

