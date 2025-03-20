namespace BudgetFlow.Application.Investments
{
    public class AssetInvestResponse
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int AssetId { get; set; }
        public int PortfolioId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
