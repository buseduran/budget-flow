using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities;

public class SummaryReport : BaseEntity
{
    public int WalletID { get; set; }
    public string Analysis { get; set; }
    public DateTime AnalysisDate { get; set; }

    public Wallet Wallet { get; set; }
}