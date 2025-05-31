using BudgetFlow.Application.Categories;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Statistics.Responses;

public class AnalysisEntryResponse
{
    public CategoryResponse Category { get; set; }
    public decimal Amount { get; set; }
}