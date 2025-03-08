using BudgetFlow.Application.AssetTypes;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface IAssetTypeRepository
    {
        Task<bool> CreateAssetTypeAsync(AssetType AssetType);
        Task<bool> UpdateAssetTypeAsync(int ID, AssetTypeDto AssetType);
        Task<bool> DeleteAssetTypeAsync(int ID);
        Task<IEnumerable<AssetTypeResponse>> GetAssetTypesAsync();
    }
}
