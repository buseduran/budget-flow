using BudgetFlow.Application.Categories;
using BudgetFlow.Application.Common.Interfaces.Repositories;
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
        int walletID)
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
                Amount = e.Value.CurrentAmount
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
                Amount = e.Value.CurrentAmount
            })
            .ToList();

        #region Calculate Trending
        var currentIncomeTotal = entryDictionary
           .Where(e => e.Key.Type == EntryType.Income)
           .Sum(e => e.Value.CurrentAmount);

        var previousIncomeTotal = entryDictionary
            .Where(e => e.Key.Type == EntryType.Income)
            .Sum(e => e.Value.PreviousAmount);

        var currentExpenseTotal = entryDictionary
            .Where(e => e.Key.Type == EntryType.Expense)
            .Sum(e => e.Value.CurrentAmount);

        var previousExpenseTotal = entryDictionary
            .Where(e => e.Key.Type == EntryType.Expense)
            .Sum(e => e.Value.PreviousAmount);

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
                Category = new CategoryResponse
                {
                    ID = e.CategoryID,
                    Name = e.Category.Name,
                    Color = e.Category.Color,
                    Type = e.Category.Type
                },
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
                CurrencyAmount = i.CurrencyAmount,
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

        var userAsset = await _context.WalletAssets
            .Where(u => u.AssetId == query.AssetID && u.WalletId == query.WalletID)
            .Select(u => new
            {
                u.Amount,
                u.Balance
            }).FirstOrDefaultAsync();

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
                TotalPrice = userAsset.Balance
            },
            AssetInvests = new PaginatedList<AssetInvestResponse>(investments, count, query.Page, query.PageSize)
        };
    }

    public async Task<List<WalletContributionResponse>> GetWalletContributionsAsync(int walletId)
    {
        var userWallets = await _context.UserWallets
            .Include(w => w.User)
            .Include(w => w.Wallet)
            .Where(w => w.WalletID == walletId)
            .ToListAsync();

        if (!userWallets.Any())
            return new List<WalletContributionResponse>();

        var contributions = new List<WalletContributionResponse>();

        // Get total wallet balance
        var wallet = await _context.Wallets
            .Where(w => w.ID == walletId)
            .Select(w => new { w.Balance })
            .FirstOrDefaultAsync();


        foreach (var userWallet in userWallets)
        {
            var userEntries = await _context.Entries
                .Include(e => e.Category)
                .Where(e => e.WalletID == walletId && e.UserID == userWallet.UserID)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            var userContribution = new WalletContributionResponse
            {
                UserID = userWallet.UserID,
                UserName = userWallet.User.Name,
                Details = userEntries.Select(e => new ContributionDetail
                {
                    Date = e.CreatedAt,
                    Amount = e.Amount,
                    Description = e.Name,
                    Type = e.Category.Type
                }).ToList()
            };

            // Calculate total contribution (since expenses are already negative)
            userContribution.TotalContribution = userContribution.Details.Sum(d => d.Amount);

            // Calculate percentage based on total wallet balance
            var contributionAmount = userContribution.TotalContribution;
            userContribution.Percentage = wallet.Balance > 0 ? contributionAmount / wallet.Balance * 100 : 0;

            contributions.Add(userContribution);
        }

        return contributions;
    }
}