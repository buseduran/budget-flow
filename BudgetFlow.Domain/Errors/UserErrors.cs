using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;

public static class UserErrors
{
    public static Error UserNotFound => new("User.NotFound", "Kullanıcı bulunamadı.");
    public static Error UserAlreadyExists => new("User.AlreadyExists", "Bu e-posta ile kayıtlı bir kullanıcı zaten var.");
    public static Error InvalidPassword => new("User.InvalidPassword", "Geçersiz şifre.");
    public static Error PasswordsDoNotMatch => new("User.PasswordsDoNotMatch", "Şifre ile şifre tekrarı uyuşmuyor.");
    public static Error CreationFailed => new("User.CreationFailed", "Kullanıcı oluşturulamadı.");
    public static Error UpdateFailed => new("User.UpdateFailed", "Kullanıcı güncellenemedi.");

    public static Error RefreshTokenRevokeFailed => new("User.RefreshTokenRevokeFailed", "Yenileme token'ı iptal edilemedi.");
    public static Error RefreshTokenCreationFailed => new("User.RefreshTokenCreationFailed", "Yenileme token'ı oluşturulamadı.");
    public static Error LogoutFailed => new("User.LogoutFailed", "Çıkış işlemi başarısız.");
    public static Error InvalidRefreshToken => new("User.InvalidRefreshToken", "Geçersiz yenileme token'ı.");
    public static Error InvalidToken => new("User.InvalidToken", "Geçersiz token.");
    public static Error RefreshTokenExpired => new("User.RefreshTokenExpired", "Yenileme token'ı süresi dolmuş.");
    public static Error RefreshTokenUpdateFailed => new("User.RefreshTokenUpdateFailed", "Yenileme token'ı güncellenemedi.");
    public static Error PasswordCannotBeEmpty => new("User.PasswordCannotBeEmpty", "Şifre boş olamaz.");
    public static Error EmailCannotBeEmpty => new("User.EmailCannotBeEmpty", "Email boş olamaz.");
    public static Error TokenAlreadyUsed => new("User.TokenAlreadyUsed", "Token zaten kullanılmış.");
    public static Error EmailConfirmationFailed => new("User.EmailConfirmationFailed", "E-posta doğrulama işlemi başarısız.");
    public static Error EmailAlreadyConfirmed => new("User.EmailAlreadyConfirmed", "E-posta doğrulama daha önce yapılmış.");
    public static Error EmailConfirmationMailFailed => new("User.EmailConfirmationMailFailed", "Kayıt başarılı ama e-posta gönderilemedi. Lütfen daha sonra tekrar deneyin.");
    public static Error LogNotFound => new("User.LogNotFound", "Log bulunamadı.");
    public static Error UserRoleCreationFailed => new("User.UserRoleCreationFailed", "Kullanıcı rolü oluşturulamadı.");
    public static Error UserNotFoundWithInvitation => new("User.UserNotFoundWithInvitation", "Bu cüzdana katılmak için lütfen davet edilen e-posta adresiyle kayıt olun.");
}
