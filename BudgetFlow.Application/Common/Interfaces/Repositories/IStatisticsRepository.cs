using BudgetFlow.Application.Investments;
using BudgetFlow.Application.Statistics.Queries.GetAssetInvestPagination;
using BudgetFlow.Application.Statistics.Responses;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;

public interface IStatisticsRepository
{
    Task<AnalysisEntriesResponse> GetAnalysisEntriesAsync(string Range, int walletID);
    Task<List<LastEntryResponse>> GetLastEntriesAsync(int walletId);
    Task<List<AssetRevenueResponse>> GetAssetRevenueAsync(string portfolio);
    Task<PaginatedAssetInvestResponse> GetAssetInvestsPaginationAsync(GetAssetInvestPaginationQuery query);
    Task<List<WalletContributionResponse>> GetWalletContributionsAsync(int walletId);
    Task<PortfolioAssetResponse> GetAssetInvestmentsAsync(int portfolioID, int userID);
    Task<List<InvestmentPaginationResponse>> GetLastInvestmentsAsync(int walletId);
}