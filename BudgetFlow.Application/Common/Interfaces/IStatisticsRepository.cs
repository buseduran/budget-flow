using BudgetFlow.Application.Statistics.Queries.GetAnalysisEntries;
using BudgetFlow.Application.Statistics.Queries.GetLastEntries;
using BudgetFlow.Application.Statistics.Queries.GetAssetRevenue;
using BudgetFlow.Application.Statistics.Queries.GetAssetInvestPagination;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Interfaces;

public interface IStatisticsRepository
{
    Task<AnalysisEntriesResponse> GetAnalysisEntriesAsync(int userID,
        string Range,
        CurrencyType currencyType,
        int walletID,
        decimal exchangeRateToTRY,
        bool convertToTRY);
    Task<List<LastEntryResponse>> GetLastEntriesAsync(int walletId);
    Task<List<AssetRevenueResponse>> GetAssetRevenueAsync(string portfolio);
    Task<PaginatedAssetInvestResponse> GetAssetInvestsPaginationAsync(GetAssetInvestPaginationQuery query);
}