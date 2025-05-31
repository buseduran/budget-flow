using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Statistics.Responses;

public class WalletContributionResponse
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public decimal TotalContribution { get; set; }
    public decimal Percentage { get; set; }
    public List<ContributionDetail> Details { get; set; } = new();
}

public class ContributionDetail
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public EntryType Type { get; set; }
}