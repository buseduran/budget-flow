using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public class InvitationErrors
{
    public static Error NotFound => new("Invitation.NotFound", "Davet bulunamadı.");
    public static Error AlreadyUsed => new("Invitation.AlreadyUsed", "Davet zaten kullanıldı.");
    public static Error Expired => new("Invitation.Expired", "Davet süresi dolmuş.");
    public static Error InvalidToken => new("Invitation.InvalidToken", "Davet geçersiz");
}
