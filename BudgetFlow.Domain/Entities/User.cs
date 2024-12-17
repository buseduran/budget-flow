﻿using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public IEnumerable<Budget> Budgets { get; set; }
        public IEnumerable<Log> Logs { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}