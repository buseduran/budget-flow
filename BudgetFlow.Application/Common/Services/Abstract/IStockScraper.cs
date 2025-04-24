using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Services.Abstract
{
    public interface IStockScraper
    {
        Task<IEnumerable<Asset>> GetStocksAsync(int assetTypeID);
    }
}
