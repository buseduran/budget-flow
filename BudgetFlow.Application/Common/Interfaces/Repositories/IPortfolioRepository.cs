using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Portfolios;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface IPortfolioRepository
    {
        Task<bool> CreatePortfolioAsync(Portfolio Portfolio);
        Task<bool> DeletePortfolioAsync(int ID);
        Task<bool> UpdatePortfolioAsync(int ID, PortfolioDto Portfolio);
        Task<List<PortfolioResponse>> GetPortfoliosAsync(int UserID);
        Task<PortfolioResponse> GetPortfolioAsync(int ID);
    }
}
