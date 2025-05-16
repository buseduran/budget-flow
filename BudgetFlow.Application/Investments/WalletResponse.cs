using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Investments;
public class WalletResponse
{
    public int ID { get; set; }
    public decimal Balance { get; set; }
    public CurrencyType Currency { get; set; }
    public WalletRole Role { get; set; }
}
