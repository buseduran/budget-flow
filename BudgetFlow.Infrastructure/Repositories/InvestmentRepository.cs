using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;
public class InvestmentRepository : IInvestmentRepository
{
    private readonly BudgetContext context;
    public InvestmentRepository(BudgetContext context)
    {
        this.context = context;
    }

    public async Task<bool> CreateInvestmentAsync(Investment Investment, bool saveChanges = true)
    {
        Investment.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        Investment.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        await context.Investments.AddAsync(Investment);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteInvestmentAsync(int ID)
    {
        var investment = await context.Investments.FindAsync(ID);
        if (investment is null) return false;

        context.Investments.Remove(investment);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateInvestmentAsync(int ID, InvestmentDto Investment)
    {
        var investment = await context.Investments.FindAsync(ID);
        if (investment is null) return false;

        investment.UnitAmount = Investment.UnitAmount;
        investment.CurrencyAmount = Investment.CurrencyAmount;
        investment.AmountInTRY = Investment.AmountInTRY;
        investment.ExchangeRate = Investment.ExchangeRate;
        investment.Description = Investment.Description;
        investment.Date = Investment.Date;

        investment.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<PaginatedList<InvestmentPaginationResponse>> GetInvestmentsAsync(int Page, int PageSize, int PortfolioID)
    {
        var investments = await context.Investments
            .Where(e => e.PortfolioId == PortfolioID)
            .Include(e => e.Asset)
            .Select(i => new InvestmentPaginationResponse
            {
                ID = i.ID,
                Name = i.Asset.Name,
                UnitAmount = i.UnitAmount,
                Unit = i.Asset.Unit,
                CurrencyAmount = i.CurrencyAmount,
                AmountInTRY = i.AmountInTRY,
                Type = i.Type,
                Currency = i.Currency,
                ExchangeRate = i.ExchangeRate,
                Description = i.Description,
                Date = i.Date,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
            }).ToListAsync();
        var count = investments.Count();
        return new PaginatedList<InvestmentPaginationResponse>(investments, count, Page, PageSize);
    }

    public async Task<PortfolioAssetResponse> GetAssetInvestmentsAsync(string portfolio, int userID)
    {
        var portfolioId = await context.Portfolios
            .Where(p => p.Name == portfolio)
            .Select(p => new
            {
                p.ID
            }).FirstOrDefaultAsync();

        var walletID = await context.Portfolios
           .Where(p => p.Name == portfolio)
           .Select(p => p.WalletID).FirstOrDefaultAsync();

        var investmentsRaw = await context.Investments
            .Where(e => e.Portfolio.Name == portfolio)
            .GroupBy(e => new
            {
                e.AssetId,
                AssetType = e.Asset.AssetType,
                AssetName = e.Asset.Name,
                e.Asset.SellPrice,
                e.Asset.Description
            })
            .Select(g => new
            {
                g.Key.AssetId,
                g.Key.AssetType,
                g.Key.AssetName,
                g.Key.SellPrice,
                g.Key.Description,
                TotalUnitAmount = g.Sum(e => e.UnitAmount),
                TotalCurrencyAmount = g.Sum(e => e.CurrencyAmount),
                Code = g.OrderByDescending(e => e.CreatedAt).First().Asset.Code,
                Unit = g.OrderByDescending(e => e.CreatedAt).First().Asset.Unit,
                Symbol = g.OrderByDescending(e => e.CreatedAt).First().Asset.Symbol,
                CreatedAt = g.Max(e => e.CreatedAt),
                UpdatedAt = g.Max(e => e.UpdatedAt)
            })
            .ToListAsync();

        var userAssets = await context.WalletAssets
            .Where(u => u.WalletId == walletID)
            .ToListAsync();

        var investments = investmentsRaw
            .Select(i =>
            {
                var userAsset = userAssets.FirstOrDefault(u => u.AssetId == i.AssetId);
                return new PortfolioAssetInvestmentsResponse
                {
                    Description = i.Description,
                    Name = i.AssetName,
                    AssetType = i.AssetType.ToString(),
                    Code = i.Code,
                    Unit = i.Unit,
                    Symbol = i.Symbol,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    AssetId = i.AssetId,
                    PortfolioId = portfolioId.ID,
                    UnitAmount = userAsset?.Amount ?? 0,
                    CurrencyAmount = userAsset?.Balance ?? 0
                };
            })
            .OrderByDescending(i => i.CreatedAt)
            .Take(5)
            .ToList();

        var portfolioAssetInfoResponse = await context.Investments
            .Where(e => e.Portfolio.Name == portfolio)
            .GroupBy(e => new { e.AssetId, e.Asset.Name, e.Asset.Code, e.Asset.Unit, e.Asset.Symbol })
            .Select(g => new PortfolioAssetInfoResponse
            {
                Name = g.Key.Name,
                Unit = g.Key.Unit
            })
            .ToListAsync();

        return new PortfolioAssetResponse
        {
            Investments = investments,
            AssetInfo = portfolioAssetInfoResponse
        };
    }

    public async Task<InvestmentResponse> GetInvestmentByIdAsync(int ID)
    {
        var investment = await context.Investments
            .Where(e => e.ID == ID)
            .Include(e => e.Asset)
            .Select(i => new InvestmentResponse
            {
                ID = i.ID,
                Name = i.Asset.Name,
                CurrencyAmount = i.CurrencyAmount,
                UnitAmount = i.UnitAmount,
                AmountInTRY = i.AmountInTRY,
                Description = i.Description,
                Date = i.Date,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                AssetID = i.AssetId,
                PortfolioID = i.PortfolioId,
                Type = i.Type,
            }).FirstOrDefaultAsync();
        return investment;
    }
}
