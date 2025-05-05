using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IWalletRepository
{
    Task<bool> CreateWalletAsync(Wallet Wallet);
    Task<bool> UpdateWalletAsync(int ID, decimal Amount);
    Task<WalletResponse> GetWalletAsync(int UserID);
    Task<bool> UpdateCurrencyAsync(int UserID, CurrencyType Currency);
    Task<CurrencyType> GetUserCurrencyAsync(int UserID);
}
