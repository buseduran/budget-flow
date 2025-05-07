using AutoMapper;
using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Categories;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;
public class BudgetRepository : IBudgetRepository
{
    private readonly BudgetContext context;
    private readonly IMapper mapper;
    public BudgetRepository(BudgetContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }
    public async Task<bool> CreateEntryAsync(Entry Entry)
    {
        Entry.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        Entry.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        Entry.Date = DateTime.SpecifyKind(Entry.Date, DateTimeKind.Utc);

        await context.Entries.AddAsync(Entry);
        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> UpdateEntryAsync(int ID, EntryDto Entry)
    {
        var entry = await context.Entries.FindAsync(ID);
        if (entry is null) return false;

        mapper.Map(Entry, entry);
        entry.Date = DateTime.SpecifyKind(entry.Date, DateTimeKind.Utc);
        entry.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteEntryAsync(int ID)
    {
        var entry = await context.Entries.FindAsync(ID);
        if (entry is null) return false;

        context.Entries.Remove(entry);
        await context.SaveChangesAsync();
        return true;
    }
    public async Task<PaginatedList<EntryResponse>> GetPaginatedAsync(int Page, int PageSize, int UserID, CurrencyType currencyType)
    {
        var entries = await context.Entries
            .OrderByDescending(c => c.CreatedAt)
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .Where(u => u.UserID == UserID)
            .Include(c => c.Category)
            .Select(e => new EntryResponse
            {
                ID = e.ID,
                Name = e.Name,
                Amount = e.Amount,
                Currency = currencyType,
                Date = e.Date,
                Category = new CategoryResponse
                {
                    ID = e.Category.ID,
                    Name = e.Category.Name,
                    Color = e.Category.Color,
                    Type = e.Category.Type
                }
            })
            .ToListAsync();
        var count = await context.Entries.CountAsync();

        return new PaginatedList<EntryResponse>(entries, count, Page, PageSize);
    }

    public async Task<AnalysisEntriesResponse> GetAnalysisEntriesAsync(int userID, string Range, CurrencyType currencyType)
    {
        var startDate = GetDateForRange.GetStartDateForRange(Range);
        var endDate = DateTime.UtcNow;
        var previousStartDate = GetDateForRange.GetPreviousStartDateForRange(Range);
        var previousEndDate = startDate.AddDays(-1);

        var groupedEntries = await context.Entries
            .Where(e => e.UserID == userID &&
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
            .Select(e => new AnalysisEntry
            {
                Category = new CategoryResponse
                {
                    ID = e.Value.CategoryID,
                    Name = e.Value.Name,
                    Color = e.Value.Color,
                    Type = e.Key.Type
                },
                Currency = currencyType,
                Amount = e.Value.CurrentAmount
            })
            .ToList();

        var expenses = entryDictionary
            .Where(e => e.Key.Type == EntryType.Expense)
            .Select(e => new AnalysisEntry
            {
                Category = new CategoryResponse
                {
                    ID = e.Value.CategoryID,
                    Name = e.Value.Name,
                    Color = e.Value.Color,
                    Type = e.Key.Type
                },
                Currency = currencyType,
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

    public async Task<List<LastEntryResponse>> GetLastFiveEntriesAsync(int userID, CurrencyType currencyType)
    {
        var entries = await context.Entries
            .Where(e => e.UserID == userID)
            .Include(e => e.Category)
            .OrderByDescending(e => e.CreatedAt)
            .Take(5)
            .Include(c => c.Category)
            .Select(e => new LastEntryResponse
            {
                ID = e.ID,
                Name = e.Name,
                Amount = e.Amount,
                Currency = currencyType,
                Date = e.Date,
                Category = new CategoryResponse
                {
                    ID = e.Category.ID,
                    Name = e.Category.Name,
                    Color = e.Category.Color,
                    Type = e.Category.Type
                }
            })
            .ToListAsync();

        return entries;
    }

    public async Task<bool> CheckEntryByCategoryAsync(int CategoryID)
    {
        return await context.Entries
            .AnyAsync(e => e.CategoryID == CategoryID);
    }

    public async Task<EntryResponse> GetEntryByIdAsync(int ID)
    {
        var entry = await context.Entries
            .Include(e => e.Category)
            .Where(e => e.ID == ID)
            .Select(e => new EntryResponse
            {
                Amount = e.Amount,
                Category = new CategoryResponse
                {
                    Type = e.Category.Type
                }
            })
            .FirstOrDefaultAsync();
        return entry;
    }
}
