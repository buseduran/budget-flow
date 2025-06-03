using AutoMapper;
using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Categories;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
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
       int walletID)
    {
        var query = context.Entries
            .Where(e => e.WalletID == walletID)
            .OrderByDescending(e => e.CreatedAt)
            .Include(e => e.Category);

        var count = await query.CountAsync();

        var entries = await query
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .Select(e => new EntryResponse
            {
                ID = e.ID,
                Name = e.Name,
                Amount = e.Amount,
                Date = e.Date,
                Category = new CategoryResponse
                {
                    ID = e.Category.ID,
                    Name = e.Category.Name,
                    Color = e.Category.Color,
                    Type = e.Category.Type
                },
                UserName= e.User.Name,
                WalletID = walletID,
            })
            .ToListAsync();

        return new PaginatedList<EntryResponse>(entries, count, Page, PageSize);
    }


    public async Task<bool> CheckEntryByCategoryAsync(int CategoryID, int UserID)
    {
        return await context.Entries
            .AnyAsync(e => e.CategoryID == CategoryID && e.UserID == UserID);
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
            })
            .FirstOrDefaultAsync();
        return entry;
    }
}
