using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;
public class WalletAssetRepository : IWalletAssetRepository
{
    private readonly BudgetContext _context;

    public WalletAssetRepository(BudgetContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WalletAsset>> GetAllAsync()
    {
        return await _context.WalletAssets.ToListAsync();
    }

    public async Task UpdateAsync(WalletAsset walletAsset)
    {
        _context.WalletAssets.Update(walletAsset);
        await _context.SaveChangesAsync();
    }
}