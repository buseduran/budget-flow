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
    public async Task<bool> CreateEntryAsync(Entry Entry, bool saveChanges = true)
    {
        Entry.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        Entry.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        Entry.Date = DateTime.SpecifyKind(Entry.Date, DateTimeKind.Utc);

        await context.Entries.AddAsync(Entry);
        if (saveChanges)
            await context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> UpdateEntryAsync(int ID, Entry Entry, bool saveChanges = true)
    {
        var entry = await context.Entries.FindAsync(ID);
        if (entry is null) return false;

        entry.Name = Entry.Name;
        entry.Amount = Entry.Amount;
        entry.AmountInTRY = Entry.AmountInTRY;
        entry.ExchangeRate = Entry.ExchangeRate;
        entry.Currency = Entry.Currency;
        entry.Date = DateTime.SpecifyKind(Entry.Date, DateTimeKind.Utc);
        entry.CategoryID = Entry.CategoryID;
        entry.WalletID = Entry.WalletID;
        entry.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteEntryAsync(int ID, bool saveChanges = true)
    {
        var entry = await context.Entries.FindAsync(ID);
        if (entry is null) return false;

        context.Entries.Remove(entry);
        if (saveChanges)
            await context.SaveChangesAsync();
        return true;
    }
    public async Task<PaginatedList<EntryResponse>> GetPaginatedAsync(
        int Page,
        int PageSize,
        int UserID,
        CurrencyType currencyType,
        int walletID)
    {
        var entries = await context.Entries
            .OrderByDescending(c => c.CreatedAt)
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .Where(u => u.UserID == UserID && u.WalletID == walletID)
            .Include(c => c.Category)
            .Select(e => new EntryResponse
            {
                ID = e.ID,
                Name = e.Name,
                Amount = e.Amount,
                AmountInTRY = e.AmountInTRY,
                ExchangeRate = e.ExchangeRate,
                Currency = currencyType,
                Date = e.Date,
                Category = new CategoryResponse
                {
                    ID = e.Category.ID,
                    Name = e.Category.Name,
                    Color = e.Category.Color,
                    Type = e.Category.Type
                },
                WalletID = walletID,
            })
            .ToListAsync();
        var count = await context.Entries.CountAsync();

        return new PaginatedList<EntryResponse>(entries, count, Page, PageSize);
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
                    ID = e.CategoryID,
                    Type = e.Category.Type
                },
                WalletID = e.WalletID,
                Currency = e.Currency
            })
            .FirstOrDefaultAsync();
        return entry;
    }
}
