using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IBudgetRepository
{
    Task<bool> CreateEntryAsync(Entry Entry, bool saveChanges = true);
    Task<bool> UpdateEntryAsync(int ID, Entry Entry, bool saveChanges = true);
    Task<bool> DeleteEntryAsync(int ID, bool saveChanges = true);
    Task<PaginatedList<EntryResponse>> GetPaginatedAsync(int Page, int PageSize, int walletID);
    Task<bool> CheckEntryByCategoryAsync(int CategoryID, int UserID);
    Task<EntryResponse> GetEntryByIdAsync(int ID);
}
