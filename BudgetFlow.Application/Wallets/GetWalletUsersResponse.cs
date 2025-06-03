using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Wallets;
public class GetWalletUsersResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public WalletRole Role { get; set; }
}