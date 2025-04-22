using AutoMapper;
using BudgetFlow.Application.AssetTypes;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;
public class AssetTypeRepository : IAssetTypeRepository
{
    private readonly BudgetContext context;
    private readonly IMapper mapper;
    public AssetTypeRepository(BudgetContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<bool> CreateAssetTypeAsync(AssetType AssetType)
    {
        AssetType.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        AssetType.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        await context.AssetTypes.AddAsync(AssetType);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAssetTypeAsync(int ID, AssetTypeDto AssetType)
    {
        var assetType = await context.AssetTypes.FindAsync(ID);
        if (assetType is null) return false;

        mapper.Map(AssetType, assetType);
        assetType.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAssetTypeAsync(int ID)
    {
        return await context.AssetTypes
              .Where(e => e.ID == ID)
              .ExecuteDeleteAsync() > 0;
    }

    public async Task<IEnumerable<AssetTypeResponse>> GetAssetTypesAsync()
    {
        var assetTypes = await context.AssetTypes
            .Select(e => new AssetTypeResponse
            {
                ID = e.ID,
                Name = e.Name,
                Description = e.Description
            }).ToListAsync();
        return assetTypes;
    }
}
