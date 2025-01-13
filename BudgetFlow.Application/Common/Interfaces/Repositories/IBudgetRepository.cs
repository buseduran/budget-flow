using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface IBudgetRepository
    {
        Task<bool> CreateEntryAsync(EntryDto Entry);
        Task<bool> UpdateEntryAsync(int ID, EntryModel Entry);
        Task<bool> DeleteEntryAsync(int ID);
        Task<PaginatedList<EntryResponse>> GetPaginatedAsync(int Page, int PageSize);
    }
}
