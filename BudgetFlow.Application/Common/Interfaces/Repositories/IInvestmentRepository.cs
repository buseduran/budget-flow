using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface IInvestmentRepository
    {
        Task<bool> CreateInvestmentAsync(Investment investment);
        Task<bool> DeleteInvestmentAsync(int ID);
        Task<bool> UpdateInvestmentAsync(int ID, InvestmentDto investment);
        Task<List<InvestmentResponse>> GetInvestmentsAsync(int PortfolioID);
        Task<List<LastInvestmentResponse>> GetLastInvestmentsAsync(string Portfolio);
    }
}
