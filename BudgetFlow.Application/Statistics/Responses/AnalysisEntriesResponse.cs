namespace BudgetFlow.Application.Statistics.Responses;

public class AnalysisEntriesResponse
{
    public List<AnalysisEntryResponse> Incomes { get; set; } = new();
    public List<AnalysisEntryResponse> Expenses { get; set; } = new();
    public decimal? IncomeTrendPercentage { get; set; }
    public decimal? ExpenseTrendPercentage { get; set; }
}