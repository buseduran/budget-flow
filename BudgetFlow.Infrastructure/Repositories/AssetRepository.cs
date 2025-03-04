using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;

namespace BudgetFlow.Infrastructure.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly BudgetContext context;
        private readonly IMapper mapper;
        public AssetRepository(BudgetContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<bool> CreateAssetAsync(Asset Asset)
        {
            Asset.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            Asset.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            await context.Assets.AddAsync(Asset);
            return await context.SaveChangesAsync() > 0;
        }
    }
}
