using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Wallets;
public class WalletResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public WalletRole Role { get; set; }
}
