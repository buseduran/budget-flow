using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IBudgetRepository
{
    Task<bool> CreateEntryAsync(Entry Entry, bool saveChanges = true);
    Task<bool> UpdateEntryAsync(int ID, EntryDto Entry, bool saveChanges = true);
    Task<bool> DeleteEntryAsync(int ID);
    Task<PaginatedList<EntryResponse>> GetPaginatedAsync(int Page, int PageSize, int UserID, CurrencyType currencyType, int walletID);
    Task<AnalysisEntriesResponse> GetAnalysisEntriesAsync(int userID, string Range, CurrencyType currencyType, int walletID);
    Task<List<LastEntryResponse>> GetLastFiveEntriesAsync(int userID, CurrencyType currencyType, int walletID);
    Task<bool> CheckEntryByCategoryAsync(int CategoryID);
    Task<EntryResponse> GetEntryByIdAsync(int ID);
}
