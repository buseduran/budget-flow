using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class Investment : BaseEntity
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int PortfolioId { get; set; }
        public Portfolio Portfolio { get; set; }
        public int AssetId { get; set; }
        public Asset Asset { get; set; }
    }
}
