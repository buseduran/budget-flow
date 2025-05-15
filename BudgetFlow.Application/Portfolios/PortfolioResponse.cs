namespace BudgetFlow.Application.Portfolios;
public class PortfolioResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal TotalInvested { get; set; }
    public int WalletID { get; set; }
}
