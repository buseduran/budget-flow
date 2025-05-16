using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IInvestmentRepository
{
    Task<bool> CreateInvestmentAsync(Investment investment);
    Task<bool> DeleteInvestmentAsync(int ID);
    Task<bool> UpdateInvestmentAsync(int ID, InvestmentDto investment);
    Task<List<InvestmentResponse>> GetInvestmentsAsync(int PortfolioID);
    Task<PortfolioAssetResponse> GetAssetInvestmentsAsync(string Portfolio, int userID);
    Task<List<Dictionary<string, object>>> GetAssetRevenueAsync(string Portfolio, int UserID);
    Task<PaginatedAssetInvestResponse> GetAssetInvestPaginationAsync(int WalletID, int PortfolioID, int AssetID, int Page, int PageSize);
}
