using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class BaseErrors
{
    public static Error UnknownError => new("Bilinmeyen bir hata oluştu.");
}
