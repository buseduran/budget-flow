using AutoMapper;
using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

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
        public async Task<bool> UpdateAssetAsync(int ID, AssetDto Asset)
        {
            var asset = await context.Assets.FindAsync(ID);
            if (asset is null) return false;

            mapper.Map(Asset, asset);
            asset.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            return await context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAssetAsync(int ID)
        {
            return await context.Assets
                    .Where(e => e.ID == ID)
                    .ExecuteDeleteAsync() > 0;
        }

        public async Task<List<AssetResponse>> GetAssetsAsync()
        {
            var assets = await context.Assets
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            var count = await context.Assets.CountAsync();

            var assetsResponse = mapper.Map<List<AssetResponse>>(assets);
            return assetsResponse;
        }

        public async Task<AssetResponse> GetAssetAsync(int ID)
        {
            var asset = await context.Assets.FindAsync(ID);
            return mapper.Map<AssetResponse>(asset);
        }

        public async Task<(decimal BuyPrice, decimal SellPrice)> GetAssetRateAsync(int ID)
        {
            if (ID == 0)
                return (0, 0);
            var rate = await context.Assets
                  .Where(e => e.ID == ID)
                  .Select(e => new
                  {
                      e.BuyPrice,
                      e.SellPrice
                  })
                  .FirstOrDefaultAsync();

            return (rate.BuyPrice, rate.SellPrice);
        }

        public async Task<UserAssetResponse> GetUserAssetAsync(int UserID, int AssetID)
        {
            var userAsset = await context.UserAssets
                .Where(e => e.UserId == UserID && e.AssetId == AssetID)
                .Select(e => new UserAssetResponse
                {
                    Amount = e.Amount,
                    Balance = e.Balance,
                    UserId = e.UserId,
                    AssetId = e.AssetId
                })
                .FirstOrDefaultAsync();
            return userAsset;
        }
        public async Task<bool> CreateUserAssetAsync(UserAsset userAsset)
        {
            userAsset.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            userAsset.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            await context.UserAssets.AddAsync(userAsset);
            return await context.SaveChangesAsync() > 0;
        }
    }
}
