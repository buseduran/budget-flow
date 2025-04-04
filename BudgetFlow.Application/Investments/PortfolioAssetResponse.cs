﻿namespace BudgetFlow.Application.Investments
{
    public class PortfolioAssetResponse
    {
        public int ID { get; set; }
        public int AssetId { get; set; }
        public string AssetType { get; set; }
        public int PortfolioId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal CurrencyAmount { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal Balance { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public string Symbol { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
