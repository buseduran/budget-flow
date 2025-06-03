using AutoMapper;
using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
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
        if (saveChanges)
            await context.SaveChangesAsync();
        return true;
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

    public async Task<PaginatedList<AssetResponse>> GetAssetsAsync(int page, int pageSize, string search = null, AssetType? assetType = null)
    {
        var query = context.Assets.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(a => EF.Functions.ILike(a.Name, $"%{search}%"));
        }

        if (assetType.HasValue)
        {
            query = query.Where(a => a.AssetType == assetType.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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

        return new PaginatedList<AssetResponse>(items, totalCount, page, pageSize);
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

    public async Task<IEnumerable<Asset>> GetAllAsync()
    {
        return await context.Assets.ToListAsync();
    }

    public async Task<Asset> GetByIdAsync(int id)
    {
        return await context.Assets.FindAsync(id);
    }

    public async Task<decimal> GetCurrentValueAsync(int id)
    {
        var asset = await context.Assets.FindAsync(id);
        return asset?.SellPrice ?? 0;
    }

    public async Task UpdateAsync(Asset asset)
    {
        context.Assets.Update(asset);
        await context.SaveChangesAsync();
    }
}
