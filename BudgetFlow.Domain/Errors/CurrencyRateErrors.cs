using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public class CurrencyRateErrors
{
    public static Error CreationFailed => new("CurrencyRate.CreationFailed", "Döviz kuru verisi oluşturulamadı.");
    public static Error DeletionFailed => new("CurrencyRate.DeletionFailed", "Döviz kuru verisi silinemedi.");
    public static Error FetchFailed => new("CurrencyRate.FetchFailed", "Güncel döviz kuru verisi alınamadı.");
}
