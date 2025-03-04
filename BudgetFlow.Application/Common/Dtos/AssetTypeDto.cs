using BudgetFlow.Domain.Common;

namespace BudgetFlow.Application.Common.Dtos
{
    public class AssetTypeDto : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
