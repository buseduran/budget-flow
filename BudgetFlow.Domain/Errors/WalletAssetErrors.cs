using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class WalletAssetErrors
{
    public static Error CreationFailed => new("Cüzdan varlığı oluşturulamadı.");
    public static Error UserAssetNotFound => new("Cüzdan varlığı bulunamadı.");
    public static Error UserAssetUpdateFailed => new("Cüzdan varlığı güncellenemedi.");
    public static Error UserAssetDeletionFailed => new("Cüzdan varlığı silinemedi.");
    public static Error InsufficientBalance => new("Yetersiz bakiye.");
    public static Error NoBalanceForAsset => new("Bu varlık için bakiye bulunmamaktadır.");
    public static Error NotEnoughAssetAmount => new("Yeterli varlık miktarı bulunmamaktadır.");

}

