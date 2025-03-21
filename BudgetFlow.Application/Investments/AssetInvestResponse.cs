using BudgetFlow.Application.Common.Utils;

namespace BudgetFlow.Application.Investments
{
    public class PaginatedAssetInvestResponse
    {
        public AssetInvestInfoResponse AssetInfo { get; set; }
        public PaginatedList<AssetInvestResponse> AssetInvests { get; set; }
    }
    public class AssetInvestResponse
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Description { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class AssetInvestInfoResponse
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public string Symbol { get; set; }
    }

}
