using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IBudgetRepository
{
    Task<bool> CreateEntryAsync(Entry Entry);
    Task<bool> UpdateEntryAsync(int ID, EntryDto Entry);
    Task<bool> DeleteEntryAsync(int ID);
    Task<PaginatedList<EntryResponse>> GetPaginatedAsync(int Page, int PageSize, int UserID, CurrencyType currencyType);
    Task<AnalysisEntriesResponse> GetAnalysisEntriesAsync(int userID, string Range, CurrencyType currencyType);
    Task<List<LastEntryResponse>> GetLastFiveEntriesAsync(int userID, CurrencyType currencyType);
    Task<bool> CheckEntryByCategory(int CategoryID);
}
