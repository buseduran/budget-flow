using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IWalletUserRepository
{
    Task<WalletUser> GetByWalletIdAndUserIdAsync(int walletId, int userId);
    Task<List<WalletUser>> GetUsersByWalletIdAsync(int walletId);
    Task<bool> AddUserToWalletAsync(WalletUser walletUser);
    Task<bool> RemoveUserFromWalletAsync(int walletId, int userId);
}