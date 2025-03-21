﻿namespace BudgetFlow.Application.Portfolios
{
    public class PortfolioResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //get investments analysis here
        public decimal TotalInvested { get; set; }
        public decimal TotalProfit { get; set; }
    }
}
