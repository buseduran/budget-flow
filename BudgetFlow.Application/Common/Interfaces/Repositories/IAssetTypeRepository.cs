using BudgetFlow.Application.Common.Dtos;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface IAssetTypeRepository
    {
        Task<bool> CreateAssetTypeAsync(AssetTypeDto AssetType);
        Task<bool> UpdateAssetTypeAsync(int ID, AssetTypeDto AssetType);
    }
}
