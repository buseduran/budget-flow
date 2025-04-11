using AutoMapper;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Portfolios;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly BudgetContext context;
        private readonly IMapper mapper;

        public PortfolioRepository(BudgetContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<int> CreatePortfolioAsync(Portfolio Portfolio)
        {
            Portfolio.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            Portfolio.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            await context.Portfolios.AddAsync(Portfolio);
            var result = await context.SaveChangesAsync();
            if (result > 0) return Portfolio.ID;
            return 0;
        }
        public async Task<bool> DeletePortfolioAsync(int ID)
        {
            return await context.Portfolios
                    .Where(e => e.ID == ID)
                    .ExecuteDeleteAsync() > 0;
        }

        public async Task<bool> UpdatePortfolioAsync(int ID, PortfolioDto Portfolio)
        {
            var portfolio = await context.Portfolios.FindAsync(ID);
            if (portfolio is null) return false;

            portfolio.Name = Portfolio.Name;
            portfolio.Description = Portfolio.Description;

            portfolio.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<List<PortfolioResponse>> GetPortfoliosAsync(int userID)
        {
            var userAssets = await context.UserAssets
                .Where(u => u.UserId == userID)
                .ToListAsync();

            var portfolios = await context.Portfolios
                .Where(p => p.UserID == userID)
                .Include(p => p.Investments)
                .ThenInclude(i => i.Asset)
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();

            var result = portfolios.Select(p =>
            {
                var portfolioAssetIds = p.Investments
                    .Select(i => i.AssetId)
                    .Distinct()
                    .ToList();

                var matchingUserAssets = userAssets
                    .Where(ua => portfolioAssetIds.Contains(ua.AssetId))
                    .ToList();

                var totalInvested = matchingUserAssets
                    .Sum(ua => ua.Balance);

                return new PortfolioResponse
                {
                    ID = p.ID,
                    Name = p.Name,
                    Description = p.Description,
                    TotalInvested = totalInvested,
                };
            }).ToList();

            return result;
        }

        public async Task<PortfolioResponse> GetPortfolioAsync(string Name)
        {
            var portfolio = await context.Portfolios
                 .Where(e => e.Name == Name)
                 .Include(e => e.Investments)
                 .Select(e => new PortfolioResponse
                 {
                     ID = e.ID,
                     Name = e.Name,
                     Description = e.Description,
                     TotalInvested = e.Investments.Sum(i => i.UnitAmount)
                 })
                 .FirstOrDefaultAsync();
            return portfolio;
        }
    }
}
