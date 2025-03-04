using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface IAssetRepository
    {
        Task<bool> CreateAssetAsync(Domain.Entities.Asset asset);
    }
}
