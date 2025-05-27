using BudgetFlow.Application.Statistics.Queries.GetAssetInvestPagination;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;

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
    Task<List<WalletContributionResponse>> GetWalletContributionsAsync(int walletId, bool convertToTRY = false);
}