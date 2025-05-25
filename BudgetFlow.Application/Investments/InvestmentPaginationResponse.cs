using BudgetFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetFlow.Application.Investments;
public class InvestmentPaginationResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal CurrencyAmount { get; set; }
    public decimal UnitAmount { get; set; }
    public string Unit { get; set; }
    public CurrencyType Currency { get; set; }
    public decimal AmountInTRY { get; set; }
    public decimal ExchangeRate { get; set; }
    public InvestmentType Type { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
