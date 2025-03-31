using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Dtos
{
    public class InvestmentDto
    {
        public int AssetId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public InvestmentType Type { get; set; }
        public DateTime Date { get; set; }
        public int PortfolioId { get; set; }
    }
}
