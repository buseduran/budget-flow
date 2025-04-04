using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Dtos
{
    public class InvestmentDto
    {
        public int AssetId { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal CurrencyAmount { get; set; }
        public string Description { get; set; }
        public InvestmentType Type { get; set; }
        public DateTime Date { get; set; }
        public int PortfolioId { get; set; }
    }
}
