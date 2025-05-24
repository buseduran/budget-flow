using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class InvestmentErrors
{
    public static Error CreationFailed => new("Investment.CreationFailed", "Yatırım oluşturulamadı.");
    public static Error InvestmentNotFound => new("Investment.NotFound", "Yatırım bulunamadı.");
    public static Error InvestmentUpdateFailed => new("Investment.UpdateFailed", "Yatırım güncellenemedi.");
    public static Error InvestmentDeletionFailed => new("Investment.DeletionFailed", "Yatırım silinemedi.");
    public static Error InvalidInvestmentId => new("Investment.InvalidId", "Geçersiz yatırım ID'si.");
    public static Error InsufficientBalance => new("Investment.InsufficientBalance", "Yetersiz bakiye.");
    public static Error NoBalanceForAsset => new("Investment.NoBalanceForAsset", "Bu varlık için bakiye bulunmamaktadır.");
    public static Error InvalidInvestmentType => new("Investment.InvalidType", "Geçersiz yatırım türü.");
}

