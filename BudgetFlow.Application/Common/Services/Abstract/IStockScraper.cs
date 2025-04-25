using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Services.Abstract
{
    public interface IStockScraper
    {
        Task<IEnumerable<Asset>> GetStocksAsync(AssetType assetType);
    }
}
