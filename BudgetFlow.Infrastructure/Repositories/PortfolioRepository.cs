using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Portfolios;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;
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
        var portfolio = await context.Portfolios
            .Where(p => p.ID == ID)
            .Include(p => p.Investments)
            .FirstOrDefaultAsync();
        if (portfolio is null) return false;

        if (portfolio.Investments.Any())
            return false;

        context.Portfolios.Remove(portfolio);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePortfolioAsync(int ID, string Name, string Description)
    {
        var portfolio = await context.Portfolios
            .Where(p => p.ID == ID)
            .FirstOrDefaultAsync();
        if (portfolio is null) return false;

        portfolio.Name = Name;
        portfolio.Description = Description;
        portfolio.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<PaginatedList<PortfolioResponse>> GetPortfoliosAsync(int Page, int PageSize, int WalletID)
    {
        var walletAssets = await context.WalletAssets
            .Where(u => u.WalletId == WalletID)
            .ToListAsync();

        var portfolios = await context.Portfolios
            .Where(p => p.WalletID == WalletID)
            .Include(p => p.Investments)
            .ThenInclude(i => i.Asset)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync();

        var result = portfolios
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .Select(p =>
            {
                var portfolioAssetIds = p.Investments
                    .Select(i => i.AssetId)
                    .Distinct()
                    .ToList();

                var matchingUserAssets = walletAssets
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
                    WalletID = p.WalletID
                };
            }).ToList();
        var count = await context.Portfolios.CountAsync(p => p.WalletID == WalletID);

        return new PaginatedList<PortfolioResponse>(result, count, Page, PageSize);
    }

    public async Task<PortfolioResponse> GetPortfolioByIdAsync(int ID)
    {
        var portfolio = await context.Portfolios
             .Where(e => e.ID == ID)
             .Include(e => e.Investments)
             .Select(e => new PortfolioResponse
             {
                 ID = e.ID,
                 Name = e.Name,
                 Description = e.Description,
                 TotalInvested = e.Investments.Sum(i => i.UnitAmount),
                 WalletID = e.WalletID,
             })
             .FirstOrDefaultAsync();
        return portfolio;
    }
}
