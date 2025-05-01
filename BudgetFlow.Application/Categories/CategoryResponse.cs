using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Categories;
public class CategoryResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public EntryType Type { get; set; }
}
