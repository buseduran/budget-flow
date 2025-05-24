using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class WalletErrors
{
    public static Error WalletNotFound => new("Wallet.NotFound", "Cüzdan bulunamadı.");
    public static Error CreationFailed => new("Wallet.CreationFailed", "Cüzdan oluşturulamadı.");
    public static Error UpdateFailed => new("Wallet.UpdateFailed", "Cüzdan bakiyesi güncellenemedi.");
    public static Error InsufficientBalance => new("Wallet.InsufficientBalance", "Cüzdan bakiyesi yetersiz.");
    public static Error NoBalanceForAsset => new("Wallet.NoBalanceForAsset", "Bu varlık için bakiye bulunmamaktadır.");
    public static Error InvalidOpeningBalance => new("Wallet.InvalidOpeningBalance", "Açılış bakiyesi 0'dan büyük olmalı.");
    public static Error UserNotFoundInWallet => new("Wallet.UserNotFoundInWallet", "Cüzdan kullanıcısı bulunamadı.");
    public static Error UserIsNotOwner => new("Wallet.UserIsNotOwner", "Bu işlemi gerçekleştirmek için gerekli yetkiye sahip değilsiniz.");
    public static Error UserWalletCreationFailed => new("Wallet.UserWalletCreationFailed", "Cüzdan kullanıcı ilişkisi oluşturulurken bir hata oluştu.");
    public static Error UserWalletAlreadyExists => new("Wallet.UserWalletAlreadyExists", "Kullanıcının zaten bir cüzdanı bulunmaktadır.");
    public static Error UserHasNoPermission => new("Wallet.UserHasNoPermission", "Bu cüzdanda bu işlemi yapma izniniz yok.");
    public static Error InvalidCurrency => new("Wallet.InvalidCurrency", "Geçersiz para birimi.");
    public static Error CurrencyRateNotFound => new("Wallet.CurrencyRateNotFound", "Para birimi kuru bulunamadı.");
}
