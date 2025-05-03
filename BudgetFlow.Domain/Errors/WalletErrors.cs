using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class WalletErrors
{
    public static Error CreationFailed => new("Cüzdan oluşturulamadı.");
    public static Error UpdateFailed => new("Cüzdan bakiyesi güncellenemedi.");
    public static Error InsufficientBalance => new("Cüzdan bakiyesi yetersiz.");
    public static Error NoBalanceForAsset => new("Bu varlık için bakiye bulunmamaktadır.");

}
