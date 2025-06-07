using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Statistics.Responses;

public class WalletContributionResponse
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public decimal TotalContribution { get; set; }
    public decimal Percentage { get; set; }
}