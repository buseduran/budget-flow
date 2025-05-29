using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Enums;
using Quartz;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Jobs;
public class MetalJob : IJob
{
    private readonly IMetalScraper _metalScraper;
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private const string CacheKey = "MetalData";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public MetalJob(
        IMetalScraper metalScraper,
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _metalScraper = metalScraper;
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await ExecuteAsync();
    }

    public async Task ExecuteAsync()
    {
        // Try to get data from cache first
        if (_cacheService.TryGetValue<IEnumerable<Asset>>(CacheKey, out var cachedAssets))
        {
            return; // Cache'de veri varsa hiçbir şey yapma
        }

        // Cache'de veri yoksa yeni veri çek ve DB'ye kaydet
        var assetType = AssetType.Metal;
        var metals = await _metalScraper.GetMetalsAsync(assetType);

        // Cache the new data
        _cacheService.Set(CacheKey, metals, CacheDuration);

        await UpdateAssets(metals);
    }

    private async Task UpdateAssets(IEnumerable<Asset> assets)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var asset in assets)
            {
                var existingAsset = await _assetRepository.GetByCodeAsync(asset.Code);
                if (existingAsset != null)
                {
                    // Update existing asset
                    var updatedAsset = new Asset
                    {
                        ID = existingAsset.ID,
                        Name = existingAsset.Name,
                        AssetType = existingAsset.AssetType,
                        BuyPrice = asset.BuyPrice,
                        SellPrice = asset.SellPrice,
                        Description = existingAsset.Description,
                        Symbol = existingAsset.Symbol,
                        Code = existingAsset.Code,
                        Unit = existingAsset.Unit,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _assetRepository.UpdateAssetAsync(updatedAsset);
                }
                else
                {
                    // Create new asset
                    asset.CreatedAt = DateTime.UtcNow;
                    asset.UpdatedAt = DateTime.UtcNow;
                    await _assetRepository.CreateAssetAsync(asset);
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
