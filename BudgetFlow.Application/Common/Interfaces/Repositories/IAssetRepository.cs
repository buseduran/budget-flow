using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface IAssetRepository
    {
        Task<bool> CreateAssetAsync(Asset asset);
        Task<bool> UpdateAssetAsync(int ID, AssetDto asset);
        Task<bool> DeleteAssetAsync(int ID);
        Task<PaginatedList<AssetResponse>> GetPaginatedAsync(int Page, int PageSize, int UserID);

    }
}
