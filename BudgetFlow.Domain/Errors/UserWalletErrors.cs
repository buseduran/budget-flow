using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class UserWalletErrors
{
    public static Error UserWalletNotFound => new("UserWallet.NotFound", "Kullanıcı cüzdanı bulunamadı");
    public static Error UserWalletCreationFailed => new("UserWallet.CreationFailed", "Cüzdan kullanıcı ilişkisi oluşturulurken bir hata oluştu");
    public static Error UserWalletAlreadyExists => new("UserWallet.AlreadyExists", "Kullanıcının zaten bir cüzdanı bulunmaktadır");
    public static Error UserWalletUpdateFailed => new("UserWallet.UpdateFailed", "Cüzdan kullanıcı ilişkisi güncellenirken bir hata oluştu");
    public static Error UserWalletDeletionFailed => new("UserWallet.DeletionFailed", "Cüzdan kullanıcı ilişkisi silinirken bir hata oluştu");
    public static Error UserWalletNoPermission => new("UserWallet.NoPermission", "Bu cüzdanda bu işlemi yapma izniniz yok");
}