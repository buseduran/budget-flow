using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;

namespace BudgetFlow.Infrastructure.Repositories
{
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
            var wallet = await context.Wallets.FindAsync(ID);
            if (wallet is null) return false;

            wallet.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            wallet.Balance += Amount;

            return await context.SaveChangesAsync() > 0;
        }
    }
}
