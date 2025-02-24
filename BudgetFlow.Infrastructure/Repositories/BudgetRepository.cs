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
                .Where(e => e.UserID == userID && e.Date >= startDate && e.Date <= endDate)
                .GroupBy(e => new { e.Category, e.Type })
                .Select(g => new
                {
                    g.Key.Category,
                    Amount = g.Sum(e => e.Amount),
                    g.Key.Type
                })
                .AsSplitQuery()
                .ToListAsync();

            var previousGroupedEntries = await context.Entries
                .Where(e => e.UserID == userID && e.Date >= previousStartDate && e.Date <= previousEndDate)
                .GroupBy(e => new { e.Category, e.Type })
                .Select(g => new
                {
                    g.Key.Category,
                    Amount = g.Sum(e => e.Amount),
                    g.Key.Type
                })
                .AsSplitQuery()
                .ToListAsync();

            var incomes = groupedEntries
                .Where(e => e.Type == EntryType.Income)
                .Select(e => new GroupedEntry { Category = e.Category, Amount = e.Amount })
                .ToList();

            var expenses = groupedEntries
                .Where(e => e.Type == EntryType.Expense)
                .Select(e => new GroupedEntry { Category = e.Category, Amount = e.Amount })
                .ToList();

            var currentIncomeTotal = groupedEntries
                .Where(t => t.Type == EntryType.Income)
                .Sum(e => e.Amount);
            var previousIncomeTotal = previousGroupedEntries
                .Where(t => t.Type == EntryType.Income)
                .Sum(e => e.Amount);

            var currentExpenseTotal = groupedEntries
                .Where(t => t.Type == EntryType.Expense)
                .Sum(e => e.Amount);
            var previousExpenseTotal = previousGroupedEntries
                .Where(t => t.Type == EntryType.Expense)
                .Sum(e => e.Amount);

            decimal incomeTrend = 0;
            if (previousIncomeTotal == 0 && currentIncomeTotal != 0)
                incomeTrend = 100;
            else
                incomeTrend = (currentIncomeTotal - previousIncomeTotal) / previousIncomeTotal * 100;


            decimal expenseTrend = 0;
            if (previousExpenseTotal == 0 && currentExpenseTotal != 0)
                expenseTrend = 100;
            else
                expenseTrend = (currentExpenseTotal - previousExpenseTotal) / previousExpenseTotal * 100;

            return new GroupedEntriesResponse { Incomes = incomes, Expenses = expenses, IncomeTrendPercentage = incomeTrend, ExpenseTrendPercentage = expenseTrend };
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
