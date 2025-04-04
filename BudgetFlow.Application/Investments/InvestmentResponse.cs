namespace BudgetFlow.Application.Investments
{
    public class InvestmentResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal CurrencyAmount { get; set; }
        public decimal UnitAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
