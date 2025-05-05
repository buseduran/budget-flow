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

    public async Task<bool> CreateWalletAsync(Wallet Wallet)
    {
        Wallet.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        Wallet.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        await context.Wallets.AddAsync(Wallet);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateWalletAsync(int ID, decimal Amount)
    {
        var wallet = await context.Wallets
            .Where(wallet => wallet.UserId == ID)
            .FirstOrDefaultAsync();
        if (wallet is null) return false;

        wallet.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        wallet.Balance += Amount;

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<WalletResponse> GetWalletAsync(int UserID)
    {
        var wallet = await context.Wallets
            .Where(wallet => wallet.UserId == UserID)
            .Select(wallet => new WalletResponse
            {
                Balance = wallet.Balance,
                Currency = wallet.Currency,
                UserId = wallet.UserId
            })
            .FirstOrDefaultAsync();
        return wallet;
    }

    public async Task<bool> UpdateCurrencyAsync(int UserID, CurrencyType Currency)
    {
        var wallet = await context.Wallets
            .Where(wallet => wallet.UserId == UserID)
            .FirstOrDefaultAsync();
        if (wallet is null) return false;
        wallet.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        wallet.Currency = Currency;
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<CurrencyType> GetUserCurrencyAsync(int UserID)
    {
        var wallet = await context.Wallets
            .Where(wallet => wallet.UserId == UserID)
            .Select(wallet => new
            {
                wallet.Currency
            })
            .FirstOrDefaultAsync();
        if (wallet is null) return CurrencyType.USD;
        return wallet.Currency;
    }
}
