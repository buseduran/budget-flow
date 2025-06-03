using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Investments;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IInvestmentRepository
{
    Task<bool> CreateInvestmentAsync(Investment investment, bool saveChanges = true);
    Task<bool> DeleteInvestmentAsync(int ID);
    Task<bool> UpdateInvestmentAsync(int ID, InvestmentDto investment);
    Task<PaginatedList<InvestmentPaginationResponse>> GetInvestmentsAsync(int Page, int PageSize, int PortfolioID, int? AssetId = null);
    Task<InvestmentResponse> GetInvestmentByIdAsync(int ID);
}
