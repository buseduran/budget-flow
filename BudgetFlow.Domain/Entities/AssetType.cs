using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class AssetType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}
