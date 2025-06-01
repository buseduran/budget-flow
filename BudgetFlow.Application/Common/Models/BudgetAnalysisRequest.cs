using BudgetFlow.Application.Statistics.Responses;

namespace BudgetFlow.Application.Common.Models;

public class BudgetAnalysisRequest
{
    public AnalysisEntriesResponse BudgetData { get; set; }
    public string AnalysisType { get; set; } // e.g., "daily", "weekly", "monthly"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}