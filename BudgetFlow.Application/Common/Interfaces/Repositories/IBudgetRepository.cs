using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IBudgetRepository
{
    Task<bool> CreateEntryAsync(Entry entry, bool saveChanges = true);
    Task<bool> UpdateEntryAsync(int ID, Entry entry, bool saveChanges = true);
    Task<bool> DeleteEntryAsync(int ID, bool saveChanges = true);
    Task<PaginatedList<EntryResponse>> GetPaginatedAsync(int page, int pageSize, int walletID, int? userID = null);
    Task<bool> CheckEntryByCategoryAsync(int categoryID, int userID);
    Task<EntryResponse> GetEntryByIdAsync(int ID);
}
