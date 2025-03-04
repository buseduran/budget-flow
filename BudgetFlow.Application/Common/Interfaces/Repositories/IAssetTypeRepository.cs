namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface IAssetTypeRepository
    {
        Task<bool> CreateAssetTypeAsync(AssetType AssetType);
    }
}
