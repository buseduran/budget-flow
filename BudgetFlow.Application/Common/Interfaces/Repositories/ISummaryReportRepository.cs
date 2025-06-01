using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;

public interface ISummaryReportRepository
{
    Task<SummaryReportResponse> GetByWalletIdAsync(int walletId);
    Task<bool> CreateOrUpdateAsync(SummaryReport report);
}