using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IAssetRepository
{
    Task<bool> CreateAssetAsync(Asset Asset);
    Task<bool> UpdateAssetAsync(Asset Asset);
    Task<bool> DeleteAssetAsync(int ID);
    Task<PaginatedList<AssetResponse>> GetAssetsAsync(int Page, int PageSize);
    Task<AssetResponse> GetAssetAsync(int ID);
    Task<AssetRateResponse> GetAssetRateAsync(int ID);
    Task<UserAssetResponse> GetUserAssetAsync(int UserID, int AssetID);
    Task<bool> CreateUserAssetAsync(UserAsset userAsset);
    Task<bool> UpdateUserAssetAsync(int ID, decimal Amount, decimal Balance);
    Task<AssetResponse> GetByCodeAsync(string AssetCode);
}
