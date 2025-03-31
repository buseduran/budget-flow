using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;

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
        public InvestmentType Type { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class AssetInvestInfoResponse
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public string Symbol { get; set; }
        public string TotalAmount { get; set; }
        public string TotalPrice { get; set; }
    }

}
