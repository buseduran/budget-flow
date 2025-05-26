using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Portfolios;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IPortfolioRepository
{
    Task<int> CreatePortfolioAsync(Portfolio Portfolio);
    Task<bool> DeletePortfolioAsync(int ID, int UserID);
    Task<bool> UpdatePortfolioAsync(int ID, string Name, string Description, int UserID);
    Task<PaginatedList<PortfolioResponse>> GetPortfoliosAsync(int Page, int PageSize, int UserID, int WalletID);
    Task<PortfolioResponse> GetPortfolioAsync(string Name, int UserID);
    Task<PortfolioResponse> GetPortfolioByIdAsync(int ID);
}
