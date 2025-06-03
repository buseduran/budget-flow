using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Investments;
using BudgetFlow.Application.Wallets;
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

    public async Task<bool> UpdateWalletAsync(int ID, decimal Amount, bool saveChanges = true)
    {
        var wallet = await context.Wallets
            .FirstOrDefaultAsync(wallet => wallet.ID == ID);
        if (wallet is null) return false;

        wallet.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        wallet.Balance += Amount;
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
            })
            .FirstOrDefaultAsync();

        return wallet;
    }

    public async Task<WalletAssetResponse> GetWalletAssetAsync(int WalletID, int AssetID)
    {
        var walletAsset = await context.WalletAssets
            .Where(wa => wa.WalletId == WalletID && wa.AssetId == AssetID)
            .Select(wa => new WalletAssetResponse
            {
                ID = wa.ID,
                Amount = wa.Amount,
                Balance = wa.Balance,
                WalletId = wa.WalletId,
                AssetId = wa.AssetId
            })
            .FirstOrDefaultAsync();

        return walletAsset;
    }

    public async Task<bool> CreateWalletAssetAsync(WalletAsset walletAsset, bool saveChanges = true)
    {
        await context.WalletAssets.AddAsync(walletAsset);
        if (saveChanges)
            await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateWalletAssetAsync(int ID, decimal Amount, decimal Balance, bool saveChanges = true)
    {
        var walletAsset = await context.WalletAssets.FindAsync(ID);
        if (walletAsset is null) return false;

        walletAsset.Amount = Amount;
        walletAsset.Balance = Balance;

        if (saveChanges)
            await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<WalletResponse>> GetWalletsAsync(int UserID)
    {
        var wallets = await context.UserWallets
            .Where(uw => uw.UserID == UserID)
            .Select(uw => new WalletResponse
            {
                ID = uw.Wallet.ID,
                Name = uw.Wallet.Name,
                Balance = uw.Wallet.Balance,
                Role = uw.Role
            })
            .ToListAsync();

        return wallets;
    }

    public async Task<IEnumerable<Wallet>> GetAllAsync()
    {
        return await context.Wallets.ToListAsync();
    }

    public async Task UpdateAsync(Wallet wallet)
    {
        context.Wallets.Update(wallet);
        await context.SaveChangesAsync();
    }

    public async Task<List<WalletAssetResponse>> GetWalletAssetsAsync(int WalletID)
    {
        var walletAssets = await context.WalletAssets
            .Where(wa => wa.WalletId == WalletID)
            .Include(wa => wa.Asset)
            .Select(wa => new WalletAssetResponse
            {
                ID = wa.ID,
                Amount = wa.Amount,
                Balance = wa.Balance,
                WalletId = wa.WalletId,
                AssetId = wa.AssetId,
                Asset = new AssetResponse
                {
                    ID = wa.Asset.ID,
                    Name = wa.Asset.Name
                }
            })
            .ToListAsync();

        return walletAssets;
    }
}
