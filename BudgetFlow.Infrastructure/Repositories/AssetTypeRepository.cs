using AutoMapper;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;

namespace BudgetFlow.Infrastructure.Repositories
{
    public class AssetTypeRepository : IAssetTypeRepository
    {
        private readonly BudgetContext context;
        private readonly IMapper mapper;
        public AssetTypeRepository(BudgetContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<bool> CreateAssetTypeAsync(AssetTypeDto AssetType)
        {
            AssetType.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            AssetType.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            AssetType assetType = mapper.Map<AssetType>(AssetType);

            await context.AssetTypes.AddAsync(assetType);
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

    }
}
