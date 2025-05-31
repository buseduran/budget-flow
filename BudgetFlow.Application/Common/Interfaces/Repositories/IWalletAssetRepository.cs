using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IWalletAssetRepository
{
    Task<IEnumerable<WalletAsset>> GetAllAsync();
    Task UpdateAsync(WalletAsset walletAsset);
}