using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Portfolios;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IPortfolioRepository
{
    Task<int> CreatePortfolioAsync(Portfolio Portfolio);
    Task<bool> DeletePortfolioAsync(int ID);
    Task<bool> UpdatePortfolioAsync(int ID, string Name, string Description);
    Task<PaginatedList<PortfolioResponse>> GetPortfoliosAsync(int Page, int PageSize, int WalletID);
    Task<PortfolioResponse> GetPortfolioByIdAsync(int ID);
}
