using BudgetFlow.Application.Common.Models;

namespace BudgetFlow.Application.Common.Services.Abstract;
public interface IGeminiService
{
    Task<string> GenerateDailyAnalysisAsync(DailyFinanceData data);
    Task<string> GenerateBudgetAnalysisAsync(BudgetAnalysisRequest request);
}
