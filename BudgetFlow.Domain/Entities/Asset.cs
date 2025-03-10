﻿using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class Asset : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int AssetTypeId { get; set; }
        public AssetType AssetType { get; set; } = null!;
        public decimal CurrentPrice { get; set; }
        public string? Description { get; set; }
        public ICollection<Investment> Investments { get; set; } = new List<Investment>();
    }
}
