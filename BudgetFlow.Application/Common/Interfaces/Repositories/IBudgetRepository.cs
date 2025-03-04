using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface IBudgetRepository
    {
        Task<bool> CreateEntryAsync(EntryEntity Entry);
        Task<bool> UpdateEntryAsync(int ID, EntryDto Entry);
        Task<bool> DeleteEntryAsync(int ID);
        Task<PaginatedList<EntryResponse>> GetPaginatedAsync(int Page, int PageSize, int UserID);
        Task<GroupedEntriesResponse> GetGroupedEntriesAsync(int userID, string Range);
        Task<List<LastEntryResponse>> GetLastFiveEntriesAsync(int userID);
    }
}
