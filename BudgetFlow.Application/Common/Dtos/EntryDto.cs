using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Dtos;
public class EntryDto
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public EntryType Type { get; set; }
    public int CategoryID { get; set; }
}
