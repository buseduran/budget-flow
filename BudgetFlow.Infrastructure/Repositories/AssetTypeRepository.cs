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

        public async Task<bool> CreateAssetTypeAsync(AssetTypeEntity AssetType)
        {
            AssetType.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            AssetType.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            await context.AssetTypes.AddAsync(AssetType);
            return await context.SaveChangesAsync() > 0;
        }
    }
}
