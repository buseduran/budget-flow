using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface IPortfolioRepository
    {
        Task<bool> CreatePortfolioAsync(Portfolio Portfolio);
    }
}
