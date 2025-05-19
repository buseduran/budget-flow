using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IWalletRepository
{
    Task<int> CreateWalletAsync(Wallet Wallet, bool saveChanges = true);
    Task<bool> UpdateWalletAsync(int ID, decimal Amount, bool saveChanges = true);
    Task<WalletResponse> GetWalletAsync(int UserID);
    Task<bool> UpdateCurrencyAsync(int UserID, CurrencyType Currency);
    Task<CurrencyType> GetUserCurrencyAsync(int UserID);
    Task<WalletAssetResponse> GetWalletAssetAsync(int WalletID, int AssetID);
    Task<bool> CreateWalletAssetAsync(WalletAsset walletAsset, bool saveChanges = true);
    Task<bool> UpdateWalletAssetAsync(int ID, decimal Amount, decimal Balance, bool saveChanges = true);
    Task<List<WalletResponse>> GetWalletsAsync(int UserID);
}
