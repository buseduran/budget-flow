using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Dtos
{
    public class InvestmentDto
    {
        public int AssetId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int PortfolioId { get; set; }
    }
}
