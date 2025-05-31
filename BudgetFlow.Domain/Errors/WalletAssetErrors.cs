using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class WalletAssetErrors
{
    public static Error CreationFailed => new("WalletAsset.CreationFailed", "Cüzdan varlığı oluşturulamadı.");
    public static Error UserAssetNotFound => new("WalletAsset.UserAssetNotFound", "Cüzdan varlığı bulunamadı.");
    public static Error UserAssetUpdateFailed => new("WalletAsset.UserAssetUpdateFailed", "Cüzdan varlığı güncellenemedi.");
    public static Error UserAssetDeletionFailed => new("WalletAsset.UserAssetDeletionFailed", "Cüzdan varlığı silinemedi.");
    public static Error NoBalanceForAsset => new("WalletAsset.NoBalanceForAsset", "Bu varlık için bakiye bulunmamaktadır.");
    public static Error NotEnoughAssetAmount => new("WalletAsset.NotEnoughAssetAmount", "Yeterli varlık miktarı bulunmamaktadır.");
    public static Error NotFound => new("WalletAsset.NotFound", "Cüzdan varlığı bulunamadı.");
}

