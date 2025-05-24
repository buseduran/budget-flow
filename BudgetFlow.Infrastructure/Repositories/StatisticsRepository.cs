using BudgetFlow.Application.Categories;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Statistics.Queries.GetAssetInvestPagination;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly BudgetContext _context;

    public StatisticsRepository(BudgetContext context)
    {
        _context = context;
    }

    public async Task<AnalysisEntriesResponse> GetAnalysisEntriesAsync(
        int userID,
        string Range,
        CurrencyType currencyType,
        int walletID,
        decimal exchangeRateToTRY,
        bool convertToTRY)
    {
        var startDate = GetDateForRange.GetStartDateForRange(Range);
        var endDate = DateTime.UtcNow;
        var previousStartDate = GetDateForRange.GetPreviousStartDateForRange(Range);
        var previousEndDate = startDate.AddDays(-1);


        var groupedEntries = await _context.Entries
            .Where(e => e.UserID == userID &&
                        e.WalletID == walletID &&
                       ((e.Date >= startDate && e.Date <= endDate) ||
                        (e.Date >= previousStartDate && e.Date <= previousEndDate)))
            .Select(e => new
            {
                e.Amount,
                e.CategoryID,
                e.Category.Name,
                e.Category.Color,
                e.Category.Type,
                Period = (e.Date >= startDate && e.Date <= endDate) ? "Current" : "Previous"
            })
            .GroupBy(e => new
            {
                e.CategoryID,
                e.Name,
                e.Color,
                e.Type,
                e.Period
            })
            .Select(g => new
            {
                g.Key.CategoryID,
                g.Key.Name,
                g.Key.Color,
                g.Key.Type,
                g.Key.Period,
                Amount = g.Sum(e => e.Amount),
            })
            .ToListAsync();

        var entryDictionary = groupedEntries
            .GroupBy(e => new { e.CategoryID, e.Name, e.Type, e.Color })
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Name = g.Key.Name,
                    Color = g.Key.Color,
                    Type = g.Key.Type,
                    CategoryID = g.Key.CategoryID,
                    CurrentAmount = g.FirstOrDefault(e => e.Period == "Current")?.Amount ?? 0,
                    PreviousAmount = g.FirstOrDefault(e => e.Period == "Previous")?.Amount ?? 0
                });

        var incomes = entryDictionary
            .Where(e => e.Key.Type == EntryType.Income)
            .Select(e => new AnalysisEntryResponse
            {
                Category = new CategoryResponse
                {
                    ID = e.Value.CategoryID,
                    Name = e.Value.Name,
                    Color = e.Value.Color,
                    Type = e.Key.Type
                },
                Currency = convertToTRY ? CurrencyType.TRY : currencyType,
                Amount = convertToTRY ? e.Value.CurrentAmount * exchangeRateToTRY : e.Value.CurrentAmount
            })
            .ToList();

        var expenses = entryDictionary
            .Where(e => e.Key.Type == EntryType.Expense)
            .Select(e => new AnalysisEntryResponse
            {
                Category = new CategoryResponse
                {
                    ID = e.Value.CategoryID,
                    Name = e.Value.Name,
                    Color = e.Value.Color,
                    Type = e.Key.Type
                },
                Currency = convertToTRY ? CurrencyType.TRY : currencyType,
                Amount = convertToTRY ? e.Value.CurrentAmount * exchangeRateToTRY : e.Value.CurrentAmount
            })
            .ToList();

        #region Calculate Trending
        var currentIncomeTotal = entryDictionary
           .Where(e => e.Key.Type == EntryType.Income)
           .Sum(e => convertToTRY ? e.Value.CurrentAmount * exchangeRateToTRY : e.Value.CurrentAmount);

        var previousIncomeTotal = entryDictionary
            .Where(e => e.Key.Type == EntryType.Income)
            .Sum(e => convertToTRY ? e.Value.PreviousAmount * exchangeRateToTRY : e.Value.PreviousAmount);

        var currentExpenseTotal = entryDictionary
            .Where(e => e.Key.Type == EntryType.Expense)
            .Sum(e => convertToTRY ? e.Value.CurrentAmount * exchangeRateToTRY : e.Value.CurrentAmount);

        var previousExpenseTotal = entryDictionary
            .Where(e => e.Key.Type == EntryType.Expense)
            .Sum(e => convertToTRY ? e.Value.PreviousAmount * exchangeRateToTRY : e.Value.PreviousAmount);

        decimal incomeTrend = previousIncomeTotal == 0 ? (currentIncomeTotal != 0 ? 100 : 0) :
            (currentIncomeTotal - previousIncomeTotal) / previousIncomeTotal * 100;

        decimal expenseTrend = previousExpenseTotal == 0 ? (currentExpenseTotal != 0 ? 100 : 0) :
            (currentExpenseTotal - previousExpenseTotal) / previousExpenseTotal * 100;
        #endregion

        return new AnalysisEntriesResponse
        {
            Incomes = incomes,
            Expenses = expenses,
            IncomeTrendPercentage = incomeTrend,
            ExpenseTrendPercentage = expenseTrend
        };
    }

    public async Task<List<LastEntryResponse>> GetLastEntriesAsync(int walletId)
    {
        var entries = await _context.Entries
            .Where(e => e.WalletID == walletId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(5)
            .Select(e => new LastEntryResponse
            {
                ID = e.ID,
                Name = e.Name,
                Amount = e.Amount,
                AmountInTRY = e.AmountInTRY,
                ExchangeRate = e.ExchangeRate,
                Currency = e.Currency,
                Date = e.Date,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .ToListAsync();

        return entries;
    }

    public async Task<List<AssetRevenueResponse>> GetAssetRevenueAsync(string portfolio)
    {
        var portfolioId = await _context.Portfolios
            .Where(p => p.Name == portfolio)
            .Select(p => p.ID)
            .FirstOrDefaultAsync();

        var investments = await _context.Investments
            .Where(e => e.PortfolioId == portfolioId)
            .GroupBy(i => new { i.Date.Date, i.Asset.Name })
            .Select(g => new AssetRevenueResponse
            {
                Date = g.Key.Date.ToString("yyyy-MM-dd"),
                Asset = g.Key.Name,
                Total = g.Sum(e => e.CurrencyAmount)
            })
            .ToListAsync();

        return investments;
    }

    public async Task<PaginatedAssetInvestResponse> GetAssetInvestsPaginationAsync(GetAssetInvestPaginationQuery query)
    {
        var investments = await _context.Investments
            .Where(e => e.PortfolioId == query.PortfolioID && e.AssetId == query.AssetID)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(i => new AssetInvestResponse
            {
                ID = i.ID,
                UnitAmount = i.UnitAmount,
                CurrencyAmount = query.ConvertToTRY ? i.AmountInTRY : i.CurrencyAmount,
                AmountInTRY = i.AmountInTRY,
                ExchangeRate = i.ExchangeRate,
                Date = i.Date,
                Type = i.Type,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToListAsync();

        var assetInvestMainResponse = await _context.Investments
            .Where(e => e.PortfolioId == query.PortfolioID && e.AssetId == query.AssetID)
            .GroupBy(e => new { e.AssetId, e.Asset.Name, e.Asset.Code, e.Asset.Unit, e.Asset.Symbol })
            .Select(g => new
            {
                ID = g.Key.AssetId,
                g.Key.Name,
                g.Key.Code,
                g.Key.Unit,
                g.Key.Symbol,
            }).FirstOrDefaultAsync();

        var wallet = await _context.Wallets
            .Where(w => w.ID == query.WalletID)
            .Select(w => new { w.Currency })
            .FirstOrDefaultAsync();

        var userAsset = await _context.WalletAssets
            .Where(u => u.AssetId == query.AssetID && u.WalletId == query.WalletID)
            .Select(u => new
            {
                u.Amount,
                u.Balance
            }).FirstOrDefaultAsync();

        decimal totalPrice = userAsset.Balance;
        if (query.ConvertToTRY && wallet.Currency != CurrencyType.TRY)
        {
            var currencyRate = await _context.CurrencyRates
                .Where(c => c.CurrencyType == wallet.Currency)
                .OrderByDescending(c => c.RetrievedAt)
                .Select(c => c.ForexSelling)
                .FirstOrDefaultAsync();

            if (currencyRate > 0)
            {
                totalPrice *= currencyRate;
            }
        }

        var count = await _context.Investments
            .Where(i => i.PortfolioId == query.PortfolioID && i.AssetId == query.AssetID)
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
                TotalPrice = totalPrice
            },
            AssetInvests = new PaginatedList<AssetInvestResponse>(investments, count, query.Page, query.PageSize)
        };
    }
}