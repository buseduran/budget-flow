using AutoMapper;
using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;
public class AssetRepository : IAssetRepository
{
    private readonly BudgetContext context;
    private readonly IMapper mapper;
    public AssetRepository(BudgetContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }
    public async Task<bool> CreateAssetAsync(Asset Asset, bool saveChanges = true)
    {
        Asset.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        Asset.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        await context.Assets.AddAsync(Asset);
        return saveChanges ? await context.SaveChangesAsync() > 0 : true;
    }
    public async Task<bool> UpdateAssetAsync(Asset Asset, bool saveChanges = true)
    {
        var asset = await context.Assets.FindAsync(Asset.ID);
        if (asset is null) return false;


        mapper.Map(Asset, asset);
        asset.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        return saveChanges ? await context.SaveChangesAsync() > 0 : true;
    }
    public async Task<bool> DeleteAssetAsync(int ID)
    {
        var asset = await context.Assets.FindAsync(ID);
        if (asset is null) return false;

        context.Assets.Remove(asset);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<PaginatedList<AssetResponse>> GetAssetsAsync(int Page, int PageSize)
    {
        var assets = await context.Assets
            .OrderByDescending(c => c.CreatedAt)
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .Select(e => new AssetResponse
            {
                ID = e.ID,
                Name = e.Name,
                AssetType = e.AssetType,
                BuyPrice = e.BuyPrice,
                SellPrice = e.SellPrice,
                Description = e.Description,
                Symbol = e.Symbol,
                Code = e.Code,
                Unit = e.Unit
            })
            .ToListAsync();
        var count = await context.Assets.CountAsync();

        return new PaginatedList<AssetResponse>(assets, count, Page, PageSize);
    }

    public async Task<AssetResponse> GetAssetAsync(int ID)
    {
        var asset = await context.Assets.FindAsync(ID);
        return mapper.Map<AssetResponse>(asset);
    }

    public async Task<AssetRateResponse> GetAssetRateAsync(int ID)
    {
        var rate = await context.Assets
              .Where(e => e.ID == ID)
              .Select(e => new AssetRateResponse
              {
                  BuyPrice = e.BuyPrice,
                  SellPrice = e.SellPrice
              })
              .FirstOrDefaultAsync();
        return rate;
    }

    //public async Task<WalletAssetResponse> GetUserAssetAsync(int UserID, int AssetID)
    //{
    //    var userAsset = await context.UserAssets
    //        .Where(e => e.UserId == UserID && e.AssetId == AssetID)
    //        .Select(e => new WalletAssetResponse
    //        {
    //            ID = e.ID,
    //            Amount = e.Amount,
    //            Balance = e.Balance,
    //            UserId = e.UserId,
    //            AssetId = e.AssetId
    //        })
    //        .FirstOrDefaultAsync();
    //    return userAsset;
    //}
    //public async Task<bool> CreateUserAssetAsync(UserAsset userAsset)
    //{
    //    userAsset.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
    //    userAsset.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
    //    await context.UserAssets.AddAsync(userAsset);
    //    return await context.SaveChangesAsync() > 0;
    //}

    //public async Task<bool> UpdateUserAssetAsync(int ID, decimal Amount, decimal Balance)
    //{
    //    var userAsset = await context.UserAssets.FindAsync(ID);
    //    userAsset.Amount = Amount;
    //    userAsset.Balance = Balance;
    //    userAsset.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
    //    return await context.SaveChangesAsync() > 0;
    //}

    public Task<AssetResponse> GetByCodeAsync(string AssetCode)
    {
        var asset = context.Assets
            .Where(e => e.Code == AssetCode)
            .Select(e => new AssetResponse
            {
                ID = e.ID,
                Name = e.Name,
                AssetType = e.AssetType,
                BuyPrice = e.BuyPrice,
                SellPrice = e.SellPrice,
                Description = e.Description,
                Symbol = e.Symbol,
                Code = e.Code,
                Unit = e.Unit
            })
            .FirstOrDefaultAsync();
        return asset;
    }
}
