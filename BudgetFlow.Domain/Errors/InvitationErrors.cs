using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public class InvitationErrors
{
    public static Error NotFound => new("Davet bulunamadı.");
    public static Error AlreadyUsed => new("Davet zaten kullanıldı.");
    public static Error Expired => new("Davet süresi dolmuş.");
    public static Error InvalidToken => new("Davet geçersiz");
}
