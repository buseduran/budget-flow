﻿using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class Portfolio : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }
        public ICollection<Investment> Investments { get; set; }
    }
}
