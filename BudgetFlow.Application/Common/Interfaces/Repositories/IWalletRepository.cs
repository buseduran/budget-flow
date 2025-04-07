using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface IWalletRepository
    {
        Task<bool> CreateWalletAsync(Wallet Wallet);
        Task<bool> UpdateWalletAsync(int ID, decimal Amount);
        Task<WalletResponse> GetWalletAsync(int UserID);
    }
}
