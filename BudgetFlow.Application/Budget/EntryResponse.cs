using BudgetFlow.Application.Categories;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Budget;
public class EntryResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public CategoryResponse Category { get; set; }
    public int WalletID { get; set; }
}