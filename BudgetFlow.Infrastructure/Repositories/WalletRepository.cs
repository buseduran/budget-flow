using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;
public class WalletRepository : IWalletRepository
{
    private readonly BudgetContext context;
    public WalletRepository(BudgetContext context)
    {
        this.context = context;
    }

    public async Task<int> CreateWalletAsync(Wallet wallet, bool saveChanges = true)
    {
        wallet.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        wallet.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        await context.Wallets.AddAsync(wallet);
        if (saveChanges)
            await context.SaveChangesAsync();
        return wallet.ID;
    }

    public async Task<bool> UpdateWalletAsync(int ID, decimal Amount, decimal AmountInTRY, bool saveChanges = true)
    {
        var wallet = await context.Wallets
            .FirstOrDefaultAsync(wallet => wallet.ID == ID);
        if (wallet is null) return false;

        wallet.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        wallet.Balance += Amount;
        wallet.BalanceInTRY += AmountInTRY;
        if (saveChanges)
            await context.SaveChangesAsync();
        return true;
    }

    public async Task<WalletResponse> GetWalletAsync(int WalletID)
    {
        var wallet = await context.Wallets
            .Where(wallet => wallet.ID == WalletID)
            .Select(wallet => new WalletResponse
            {
                ID = wallet.ID,
                Name = wallet.Name,
                Balance = wallet.Balance,
                Currency = wallet.Currency,
            })
            .FirstOrDefaultAsync();

        return wallet;
    }

    public async Task<bool> UpdateCurrencyAsync(int WalletID, CurrencyType Currency)
    {
        var wallet = await context.Wallets
            .FirstOrDefaultAsync(wallet => wallet.ID == WalletID);
        if (wallet is null) return false;

        wallet.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        wallet.Currency = Currency;

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<CurrencyType> GetUserCurrencyAsync(int WalletID)
    {
        var wallet = await context.Wallets
            .Where(wallet => wallet.ID == WalletID)
            .Select(wallet => wallet.Currency)
            .FirstOrDefaultAsync();

        return wallet;
    }

    public async Task<WalletAssetResponse> GetWalletAssetAsync(int WalletID, int AssetID)
    {
        var walletAsset = await context.WalletAssets
            .Where(e => e.WalletId == WalletID && e.AssetId == AssetID)

            .Select(e => new WalletAssetResponse
            {
                ID = e.ID,
                Amount = e.Amount,
                Balance = e.Balance,
                WalletId = e.WalletId,
                AssetId = e.AssetId
            })
            .FirstOrDefaultAsync();
        return walletAsset;
    }

    public async Task<bool> CreateWalletAssetAsync(WalletAsset walletAsset, bool saveChanges = true)
    {
        walletAsset.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        walletAsset.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        await context.WalletAssets.AddAsync(walletAsset);
        if (saveChanges)
            await context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> UpdateWalletAssetAsync(int ID, decimal Amount, decimal Balance, bool saveChanges = true)
    {
        var userAsset = await context.WalletAssets.FindAsync(ID);
        userAsset.Amount = Amount;
        userAsset.Balance = Balance;
        userAsset.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        if (saveChanges)
            await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<WalletResponse>> GetWalletsAsync(int userID)
    {
        var wallets = await context.UserWallets
            .Where(uw => uw.UserID == userID)
            .Include(uw => uw.Wallet)
            .Select(uw => new WalletResponse
            {
                ID = uw.Wallet.ID,
                Name = uw.Wallet.Name,
                Balance = uw.Wallet.Balance,
                Currency = uw.Wallet.Currency,
                Role = uw.Role
            }).ToListAsync();

        return wallets;
    }
}
