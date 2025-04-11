namespace BudgetFlow.Application.Investments
{
    public class UserAssetResponse
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public int UserId { get; set; }
        public int AssetId { get; set; }

    }
}
