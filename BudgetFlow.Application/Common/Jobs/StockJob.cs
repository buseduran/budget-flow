using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using Quartz;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Jobs;
public class StockJob : IJob
{
    private readonly IStockScraper _stockScraper;
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StockJob(
        IStockScraper stockScraper,
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork)
    {
        _stockScraper = stockScraper;
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await ExecuteAsync();
    }

    public async Task ExecuteAsync()
    {
        var assetType = AssetType.Stock;
        var stocks = await _stockScraper.GetStocksAsync(assetType);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var stock in stocks)
            {
                var existingAsset = await _assetRepository.GetByCodeAsync(stock.Code);
                if (existingAsset != null)
                {
                    // Update existing asset
                    var asset = new Asset
                    {
                        ID = existingAsset.ID,
                        Name = existingAsset.Name,
                        AssetType = existingAsset.AssetType,
                        BuyPrice = stock.BuyPrice,
                        SellPrice = stock.SellPrice,
                        Description = stock.Description,
                        Symbol = existingAsset.Symbol,
                        Code = existingAsset.Code,
                        Unit = existingAsset.Unit,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _assetRepository.UpdateAssetAsync(asset);
                }
                else
                {
                    // Create new asset
                    stock.CreatedAt = DateTime.UtcNow;
                    stock.UpdatedAt = DateTime.UtcNow;
                    await _assetRepository.CreateAssetAsync(stock);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}

