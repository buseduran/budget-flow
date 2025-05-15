using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Users;
public class UserResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsEmailConfirmed { get; set; }

    public List<UserWalletResponse> Wallets { get; set; }
}

public class UserWalletResponse
{
    public int WalletID { get; set; }
    public WalletRole Role { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; }
}