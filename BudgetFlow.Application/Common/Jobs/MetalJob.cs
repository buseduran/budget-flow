using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Scrapers.Abstract;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace BudgetFlow.Application.Common.Jobs;
public class MetalJob : IJob
{
    private readonly IMetalScraper _metalScraper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private const string CacheKey = "MetalData";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public MetalJob(
        IMetalScraper metalScraper,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _metalScraper = metalScraper;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await ExecuteAsync();
    }

    public async Task ExecuteAsync()
    {
        #region Check Cache Data
        if (_cacheService.TryGetValue<IEnumerable<Asset>>(CacheKey, out var cachedAssets))
        {
            return;
        }
        #endregion

        var assetType = AssetType.Metal;
        var metals = await _metalScraper.GetMetalsAsync(assetType);

        _cacheService.Set(CacheKey, metals, CacheDuration);
        await UpdateAssets(metals);
    }

    private async Task UpdateAssets(IEnumerable<Asset> assets)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var assetsDbSet = _unitOfWork.GetAssets();
            var existingAssets = await assetsDbSet
                .Where(a => a.AssetType == AssetType.Metal)
                .ToDictionaryAsync(a => a.Code);

            var now = DateTime.UtcNow;
            var assetsToUpdate = new List<Asset>();
            var assetsToInsert = new List<Asset>();

            #region Prepare Metal Assets for Bulk Update/Insert 
            foreach (var asset in assets)
            {
                if (existingAssets.TryGetValue(asset.Code, out var existingAsset))
                {
                    // Update existing asset
                    existingAsset.BuyPrice = asset.BuyPrice;
                    existingAsset.SellPrice = asset.SellPrice;
                    existingAsset.UpdatedAt = now;
                    assetsToUpdate.Add(existingAsset);
                }
                else
                {
                    // Prepare new asset for insert
                    asset.CreatedAt = now;
                    asset.UpdatedAt = now;
                    assetsToInsert.Add(asset);
                }
            }
            #endregion

            #region Bulk Update/Insert Assets
            if (assetsToUpdate.Any()) assetsDbSet.UpdateRange(assetsToUpdate);
            if (assetsToInsert.Any()) await assetsDbSet.AddRangeAsync(assetsToInsert);
            #endregion

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
