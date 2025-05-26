using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using Quartz;

namespace BudgetFlow.Application.Common.Jobs;
public class StockJob : IJob
{
    private readonly IStockScraper _stockScraper;
    public StockJob(IStockScraper stockScraper)
    {
        _stockScraper = stockScraper;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var assetType = AssetType.Stock;
        var stocks = await _stockScraper.GetStocksAsync(assetType);
    }
}

