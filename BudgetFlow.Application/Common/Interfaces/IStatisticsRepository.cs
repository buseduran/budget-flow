using BudgetFlow.Application.Statistics.Queries.GetAnalysisEntries;
using BudgetFlow.Application.Statistics.Queries.GetLastEntries;
using BudgetFlow.Application.Statistics.Queries.GetAssetRevenue;
using BudgetFlow.Application.Statistics.Queries.GetAssetInvestPagination;
using BudgetFlow.Application.Statistics.Responses;

namespace BudgetFlow.Application.Common.Interfaces;

public interface IStatisticsRepository
{
    Task<AnalysisEntriesResponse> GetAnalysisEntriesAsync(int walletId, string range, bool convertToTRY);
    Task<List<LastEntryResponse>> GetLastEntriesAsync(int walletId);
    Task<List<AssetRevenueResponse>> GetAssetRevenueAsync(string portfolio);
    Task<PaginatedAssetInvestResponse> GetAssetInvestsPaginationAsync(GetAssetInvestPaginationQuery query);
}