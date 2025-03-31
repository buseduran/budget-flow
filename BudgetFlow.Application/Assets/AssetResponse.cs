namespace BudgetFlow.Application.Assets
{
    public class AssetResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int AssetTypeId { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
        public string Description { get; set; }
        public string Symbol { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
    }
}
