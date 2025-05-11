using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;

public static class UserErrors
{
    public static Error UserNotFound => new("Kullanıcı bulunamadı.");
    public static Error UserAlreadyExists => new("Bu e-posta ile kayıtlı bir kullanıcı zaten var.");
    public static Error InvalidPassword => new("Geçersiz şifre.");
    public static Error PasswordsDoNotMatch => new("Şifre ile şifre tekrarı uyuşmuyor.");
    public static Error CreationFailed => new("Kullanıcı oluşturulamadı.");
    public static Error UpdateFailed => new("Kullanıcı güncellenemedi.");

    public static Error RefreshTokenRevokeFailed = new("Yenileme token'ı iptal edilemedi.");
    public static Error RefreshTokenCreationFailed => new("Yenileme token'ı oluşturulamadı.");
    public static Error LogoutFailed => new("Çıkış işlemi başarısız.");
    public static Error InvalidRefreshToken => new("Geçersiz yenileme token'ı.");
    public static Error InvalidToken => new("Geçersiz token.");
    public static Error RefreshTokenExpired => new("Yenileme token'ı süresi dolmuş.");
    public static Error RefreshTokenUpdateFailed => new("Yenileme token'ı güncellenemedi.");
    public static Error PasswordCannotBeEmpty => new("Şifre boş olamaz.");
    public static Error EmailCannotBeEmpty => new("Email boş olamaz.");
    public static Error TokenAlreadyUsed => new("Token zaten kullanılmış.");
    public static Error EmailConfirmationFailed => new("E-posta doğrulama işlemi başarısız.");
    public static Error EmailAlreadyConfirmed => new("E-posta doğrulama daha önce yapılmış.");
    public static Error EmailConfirmationMailFailed => new("Kayıt başarılı ama e-posta gönderilemedi. Lütfen daha sonra tekrar deneyin.");
    public static Error LogNotFound => new("Log bulunamadı.");
}
