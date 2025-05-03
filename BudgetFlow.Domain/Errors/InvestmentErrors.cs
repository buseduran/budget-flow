using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class InvestmentErrors
{
    public static Error CreationFailed => new("Yatırım oluşturulamadı.");
    public static Error InvestmentNotFound => new("Yatırım bulunamadı.");
    public static Error InvestmentUpdateFailed => new("Yatırım güncellenemedi.");
    public static Error InvestmentDeletionFailed => new("Yatırım silinemedi.");
    public static Error InvalidInvestmentId => new("Geçersiz yatırım ID'si.");
    public static Error InsufficientBalance => new("Yetersiz bakiye.");
    public static Error NoBalanceForAsset => new("Bu varlık için bakiye bulunmamaktadır.");
    public static Error InvalidInvestmentType => new("Geçersiz yatırım türü.");
}

