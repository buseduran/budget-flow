using AutoMapper;
using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
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
            Entry.Date = DateTime.UtcNow;
            await context.Entries.AddAsync(Entry);
            return await context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateEntryAsync(int ID, EntryModel Entry)
        {
            var entry = await context.Entries.FindAsync(ID);
            if (entry is null) return false;

            mapper.Map(Entry, entry);
            entry.Date = DateTime.SpecifyKind(entry.Date, DateTimeKind.Utc);

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
    }
}
