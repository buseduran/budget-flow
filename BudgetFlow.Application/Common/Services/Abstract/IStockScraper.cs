namespace BudgetFlow.Application.Common.Services.Abstract
{
    public interface IStockScraper
    {
        Task<IEnumerable<Stock>> GetStocksAsync();
    }
}
