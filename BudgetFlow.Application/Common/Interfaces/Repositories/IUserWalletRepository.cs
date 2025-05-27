using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IUserWalletRepository
{
    Task<bool> CreateAsync(UserWallet userWallet, bool saveChanges = true);
    Task<UserWallet> GetByWalletIdAndUserIdAsync(int walletID, int userID);
    Task<List<UserWallet>> GetUsersByWalletIdAsync(int walletID);
    Task<UserWallet> GetUserWalletByRoleAsync(int userID, WalletRole walletRole);
    Task<bool> AddUserToWalletAsync(UserWallet walletUser);
    Task<bool> RemoveUserFromWalletAsync(int walletID, int userID);
}