namespace BudgetFlow.Application.Investments
{
    public class AssetInvestmentResponse
    {
        public int ID { get; set; }
        public int AssetID { get; set; }
        public int PortfolioID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public string Symbol { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
