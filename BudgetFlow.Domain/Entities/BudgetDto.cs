using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities;
public class BudgetDto : BaseEntity
{
    public decimal NetAmount { get; set; } // incomes - expenses
    public decimal NetIncomes { get; set; } // incomes  
    public decimal NetExpenses { get; set; } // expenses
    public int Month { get; set; }
    public int Year { get; set; }

    public int UserID { get; set; }
    public UserDto User { get; set; }
}
