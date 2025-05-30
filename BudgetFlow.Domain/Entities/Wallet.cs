﻿using BudgetFlow.Domain.Common;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Domain.Entities;
public class Wallet : BaseEntity
{
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public ICollection<User> User { get; set; }
}
