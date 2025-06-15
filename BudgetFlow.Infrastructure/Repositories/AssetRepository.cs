using AutoMapper;
using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
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

    public async Task<bool> UpdateAssetAsync(Asset asset, bool saveChanges = true)
    {
        var existAsset = await context.Assets.FindAsync(asset.ID);
        if (asset is null) return false;

        mapper.Map(asset, existAsset);
        existAsset.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        return saveChanges ? await context.SaveChangesAsync() > 0 : true;
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

    public async Task<IEnumerable<Asset>> GetAllAsync()
    {
        return await context.Assets.ToListAsync();
    }

    public async Task<Asset> GetByIDAsync(int ID)
    {
        return await context.Assets.FindAsync(ID);
    }

    public async Task<decimal> GetCurrentValueAsync(int ID)
    {
        var asset = await context.Assets.FindAsync(ID);
        return asset?.SellPrice ?? 0;
    }

    public async Task UpdateAsync(Asset asset)
    {
        context.Assets.Update(asset);
        await context.SaveChangesAsync();
    }
}
