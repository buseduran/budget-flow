using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IAssetRepository
{
    Task<bool> UpdateAssetAsync(Asset asset, bool saveChanges = true);
    Task<PaginatedList<AssetResponse>> GetAssetsAsync(int page, int pageSize, string search = null, AssetType? assetType = null);
    Task<AssetResponse> GetAssetAsync(int ID);
    Task<IEnumerable<Asset>> GetAllAsync();
    Task<Asset> GetByIDAsync(int ID);
    Task<decimal> GetCurrentValueAsync(int ID);
    Task UpdateAsync(Asset asset);
}
