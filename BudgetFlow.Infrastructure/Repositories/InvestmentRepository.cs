using AutoMapper;
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
    private readonly IMapper mapper;
    public InvestmentRepository(BudgetContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
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
        investment.Description = Investment.Description;
        investment.Date = Investment.Date;

        investment.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<PaginatedList<InvestmentResponse>> GetInvestmentsAsync(int Page, int PageSize, int PortfolioID)
    {
        var investments = await context.Investments
            .Where(e => e.PortfolioId == PortfolioID)
            .Include(e => e.Asset)
            .Select(i => new InvestmentResponse
            {
                ID = i.ID,
                Name = i.Asset.Name,
                CurrencyAmount = i.CurrencyAmount,
                UnitAmount = i.UnitAmount,
                Description = i.Description,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
            }).ToListAsync();
        var count = investments.Count();
        return new PaginatedList<InvestmentResponse>(investments, count, Page, PageSize);
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

    public async Task<List<Dictionary<string, object>>> GetAssetRevenueAsync(string Portfolio, int UserID)
    {
        var portfolioId = await context.Portfolios
            .Where(p => p.Name == Portfolio && p.UserID == UserID)
            .Select(p => p.ID)
            .FirstOrDefaultAsync();

        var investments = await context.Investments
            .Where(e => e.Portfolio.ID == portfolioId)
            .GroupBy(i => new { i.Date.Date, i.Asset.Name })
            .Select(g => new
            {
                Date = g.Key.Date.ToString("yyyy-MM-dd"),
                Asset = g.Key.Name,
                Total = g.Sum(e => e.CurrencyAmount)
            }).ToListAsync();

        var transformedData = investments
            .GroupBy(i => i.Date)
            .Select(g =>
            {
                var dict = new Dictionary<string, object> { { "date", g.Key } };
                foreach (var item in g)
                {
                    dict[item.Asset] = item.Total;
                }
                return dict;
            })
            .ToList();

        return transformedData;
    }

    public async Task<PaginatedAssetInvestResponse> GetAssetInvestPaginationAsync(int WalletID, int PortfolioID, int AssetID, int Page, int PageSize)
    {
        var investments = await context.Investments
             .Where(e => e.PortfolioId == PortfolioID && e.AssetId == AssetID)
             .OrderByDescending(c => c.CreatedAt)
             .Skip((Page - 1) * PageSize)
             .Take(PageSize)
             .Select(i => new AssetInvestResponse
             {
                 ID = i.ID,
                 UnitAmount = i.UnitAmount,
                 CurrencyAmount = i.CurrencyAmount,
                 Description = i.Description,
                 Date = i.Date,
                 Type = i.Type,
                 CreatedAt = i.CreatedAt,
                 UpdatedAt = i.UpdatedAt
             })
             .ToListAsync();

        // TODO : null check
        var assetInvestMainResponse = await context.Investments
            .Where(e => e.PortfolioId == PortfolioID && e.AssetId == AssetID)
            .GroupBy(e => new { e.AssetId, e.Asset.Name, e.Asset.Code, e.Asset.Unit, e.Asset.Symbol })
            .Select(g => new
            {
                ID = g.Key.AssetId,
                g.Key.Name,
                g.Key.Code,
                g.Key.Unit,
                g.Key.Symbol,
            }).FirstOrDefaultAsync();

        var userAsset = await context.WalletAssets
            .Where(u => u.AssetId == AssetID && u.WalletId == WalletID)
            .Select(u => new
            {
                u.Amount,
                u.Balance
            }).FirstOrDefaultAsync();

        var count = await context.Investments
            .Where(i => i.PortfolioId == PortfolioID && i.AssetId == AssetID)
            .CountAsync();

        return new PaginatedAssetInvestResponse
        {
            AssetInfo = new AssetInvestInfoResponse
            {
                ID = assetInvestMainResponse.ID,
                Name = assetInvestMainResponse.Name,
                Code = assetInvestMainResponse.Code,
                Unit = assetInvestMainResponse.Unit,
                Symbol = assetInvestMainResponse.Symbol,
                TotalAmount = userAsset.Amount,
                TotalPrice = userAsset.Balance
            },
            AssetInvests = new PaginatedList<AssetInvestResponse>(investments, count, Page, PageSize)
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
