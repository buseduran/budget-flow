using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public class GeneralErrors
{
    public static Error FromMessage(string message) => new(message);
}
