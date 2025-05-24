using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class BaseErrors
{
    public static Error UnknownError => new("Base.UnknownError", "Bilinmeyen bir hata oluştu.");
}
