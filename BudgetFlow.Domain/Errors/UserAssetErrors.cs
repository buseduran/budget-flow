using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class UserAssetErrors
{
    public static Error CreationFailed => new("Kullanıcı varlığı oluşturulamadı.");
    public static Error UserAssetNotFound => new("Kullanıcı varlığı bulunamadı.");
    public static Error UserAssetUpdateFailed => new("Kullanıcı varlığı güncellenemedi.");
    public static Error UserAssetDeletionFailed => new("Kullanıcı varlığı silinemedi.");
    public static Error InsufficientBalance => new("Yetersiz bakiye.");
    public static Error NoBalanceForAsset => new("Bu varlık için bakiye bulunmamaktadır.");
    public static Error NotEnoughAssetAmount => new("Yeterli varlık miktarı bulunmamaktadır.");

}

