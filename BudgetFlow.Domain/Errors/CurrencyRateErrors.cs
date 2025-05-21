using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public class CurrencyRateErrors
{
    public static Error CreationFailed => new("Döviz kuru verisi oluşturulamadı.");
    public static Error DeletionFailed => new("Döviz kuru verisi silinemedi.");
    public static Error FetchFailed => new("Güncel döviz kuru verisi alınamadı.");
}
