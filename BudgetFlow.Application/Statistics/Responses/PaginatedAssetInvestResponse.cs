using BudgetFlow.Application.Common.Utils;

namespace BudgetFlow.Application.Statistics.Responses;

public class PaginatedAssetInvestResponse
{
    public AssetInvestInfoResponse AssetInfo { get; set; }
    public PaginatedList<AssetInvestResponse> AssetInvests { get; set; }
}