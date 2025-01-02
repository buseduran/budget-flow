using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;

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
    }
}
