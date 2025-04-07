using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface IAssetRepository
    {
        Task<bool> CreateAssetAsync(Asset Asset);
        Task<bool> UpdateAssetAsync(int ID, AssetDto Asset);
        Task<bool> DeleteAssetAsync(int ID);
        Task<List<AssetResponse>> GetAssetsAsync();
        Task<AssetResponse> GetAssetAsync(int ID);
        Task<(decimal BuyPrice, decimal SellPrice)> GetAssetRateAsync(int ID);
        Task<UserAssetResponse> GetUserAssetAsync(int UserID, int AssetID);
        Task<bool> CreateUserAssetAsync(UserAsset userAsset);
    }
}
