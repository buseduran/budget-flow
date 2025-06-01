using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Scrapers.Abstract;
public interface IStockScraper
{
    Task<IEnumerable<Asset>> GetStocksAsync(AssetType assetType);
}
