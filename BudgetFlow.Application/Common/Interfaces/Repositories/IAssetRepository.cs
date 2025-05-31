using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IAssetRepository
{
    Task<bool> CreateAssetAsync(Asset Asset, bool saveChanges = true);
    Task<bool> UpdateAssetAsync(Asset Asset, bool saveChanges = true);
    Task<bool> DeleteAssetAsync(int ID);
    Task<PaginatedList<AssetResponse>> GetAssetsAsync(int Page, int PageSize, string search = null, AssetType? assetType = null);
    Task<AssetResponse> GetAssetAsync(int ID);
    Task<AssetRateResponse> GetAssetRateAsync(int ID);
    Task<AssetResponse> GetByCodeAsync(string AssetCode);
    Task<IEnumerable<Asset>> GetAllAsync();
    Task<Asset> GetByIdAsync(int id);
    Task<decimal> GetCurrentValueAsync(int id);
    Task UpdateAsync(Asset asset);
}
