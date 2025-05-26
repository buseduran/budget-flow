using BudgetFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetFlow.Application.Users;
public class UserPaginationResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public List<UserWalletPaginationResponse> Wallets { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
public class UserWalletPaginationResponse
{
    public int WalletID { get; set; }
    public WalletRole Role { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; }
}