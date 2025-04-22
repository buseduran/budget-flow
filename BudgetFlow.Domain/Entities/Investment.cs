using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities;
public class Investment : BaseEntity
{
    public decimal CurrencyAmount { get; set; }
    public decimal UnitAmount { get; set; }
    public string Description { get; set; }
    public InvestmentType Type { get; set; }
    public DateTime Date { get; set; }
    public int PortfolioId { get; set; }
    public Portfolio Portfolio { get; set; }
    public int AssetId { get; set; }
    public Asset Asset { get; set; }
}
