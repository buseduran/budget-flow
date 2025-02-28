using AutoMapper;
using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly BudgetContext context;
        private readonly IMapper mapper;
        public BudgetRepository(BudgetContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<bool> CreateEntryAsync(EntryDto Entry)
        {
            Entry.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            Entry.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            Entry.Date = DateTime.SpecifyKind(Entry.Date, DateTimeKind.Utc);

            await context.Entries.AddAsync(Entry);
            return await context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateEntryAsync(int ID, EntryModel Entry)
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
            return await context.Entries
                    .Where(e => e.ID == ID)
                    .ExecuteDeleteAsync() > 0;
        }
        public async Task<PaginatedList<EntryResponse>> GetPaginatedAsync(int Page, int PageSize, int UserID)
        {
            var entries = await context.Entries
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .Where(u => u.UserID == UserID)
                .ToListAsync();
            var count = await context.Entries.CountAsync();

            var entriesResponse = mapper.Map<List<EntryResponse>>(entries);

            return new PaginatedList<EntryResponse>(entriesResponse, count, Page, PageSize);
        }

        public async Task<GroupedEntriesResponse> GetGroupedEntriesAsync(int userID, string Range)
        {
            var startDate = GetDateForRange.GetStartDateForRange(Range);
            var endDate = DateTime.UtcNow;
            var previousStartDate = GetDateForRange.GetPreviousStartDateForRange(Range);
            var previousEndDate = startDate.AddDays(-1);

            var groupedEntries = await context.Entries
                .Where(e => e.UserID == userID &&
                           ((e.Date >= startDate && e.Date <= endDate) || (e.Date >= previousStartDate && e.Date <= previousEndDate)))
                .Include(c => c.Category)
                .GroupBy(e => new { e.CategoryID, e.Category.Color, e.Type, Period = e.Date >= startDate ? "Current" : "Previous" })
                .Select(g => new
                {
                    g.Key.CategoryID,
                    g.Key.Color,
                    g.Key.Type,
                    g.Key.Period,
                    Amount = g.Sum(e => e.Amount),
                })
                .ToListAsync();

            var entryDictionary = groupedEntries
                .GroupBy(e => new { e.CategoryID, e.Type, e.Color })
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        Color = g.Key.Color,
                        CurrentAmount = g.FirstOrDefault(e => e.Period == "Current")?.Amount ?? 0,
                        PreviousAmount = g.FirstOrDefault(e => e.Period == "Previous")?.Amount ?? 0
                    });

            var incomes = entryDictionary
                .Where(e => e.Key.Type == EntryType.Income)
                .Select(e => new GroupedEntry { CategoryID = e.Key.CategoryID, Color = e.Value.Color, Amount = e.Value.CurrentAmount })
                .ToList();

            var expenses = entryDictionary
                .Where(e => e.Key.Type == EntryType.Expense)
                .Select(e => new GroupedEntry { CategoryID = e.Key.CategoryID, Color = e.Value.Color, Amount = e.Value.CurrentAmount })
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

            return new GroupedEntriesResponse
            {
                Incomes = incomes,
                Expenses = expenses,
                IncomeTrendPercentage = incomeTrend,
                ExpenseTrendPercentage = expenseTrend
            };
        }

        public async Task<List<EntryResponse>> GetLastFiveEntriesAsync(int userID)
        {
            var entries = await context.Entries
                .Where(e => e.UserID == userID)
                .OrderByDescending(e => e.CreatedAt)
                .Take(5)
                .ToListAsync();
            return mapper.Map<List<EntryResponse>>(entries);
        }
    }
}
