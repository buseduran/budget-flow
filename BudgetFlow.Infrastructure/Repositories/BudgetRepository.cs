using AutoMapper;
using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Budget.Queries.GetEntryPagination;
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

            var entryDto = mapper.Map<EntryDto>(Entry);
            context.Entries.Update(entryDto);
            return await context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteEntryAsync(int ID)
        {
            var entry = await context.Entries.FindAsync(ID);
            if (entry == null) return false;

            context.Entries.Remove(entry);
            return await context.SaveChangesAsync() > 0;
        }
        public async Task<PaginatedList<EntryResponse>> GetPaginatedAsync(int Page,int PageSize)
        {
            var entries = await context.Entries
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var count = await context.Entries.CountAsync();
            var entriesResponse = mapper.Map<List<EntryResponse>>(entries);
            return new PaginatedList<EntryResponse>(entriesResponse, count, Page, PageSize);
        }
    }
}
