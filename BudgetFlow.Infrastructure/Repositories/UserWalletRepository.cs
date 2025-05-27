using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;
public class UserWalletRepository : IUserWalletRepository
{
    private readonly BudgetContext context;
    public UserWalletRepository(BudgetContext context)
    {
        this.context = context;
    }
    public async Task<bool> CreateAsync(UserWallet userWallet, bool saveChanges = true)
    {
        userWallet.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        userWallet.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        await context.UserWallets.AddAsync(userWallet);
        if (saveChanges)
            await context.SaveChangesAsync();
        return true;
    }
    public Task<bool> AddUserToWalletAsync(UserWallet userWallet)
    {
        throw new NotImplementedException();
    }

    public async Task<UserWallet> GetByWalletIdAndUserIdAsync(int walletID, int userID)
    {
        var userWallet = await context.UserWallets
            .Include(uw => uw.Wallet)
            .FirstOrDefaultAsync(uw => uw.WalletID == walletID && uw.UserID == userID);
        return userWallet;
    }

    public Task<List<UserWallet>> GetUsersByWalletIdAsync(int walletID)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveUserFromWalletAsync(int walletID, int userID)
    {
        throw new NotImplementedException();
    }

    public Task<UserWallet> GetUserWalletByRoleAsync(int userID, WalletRole walletRole)
    {
        var userWallet = context.UserWallets
            .Include(uw => uw.Wallet)
            .FirstOrDefaultAsync(uw => uw.UserID == userID && uw.Role == walletRole);
        return userWallet;
    }
}
