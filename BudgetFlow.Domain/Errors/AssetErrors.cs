using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;

public static class AssetErrors
{
    public static Error CreationFailed => new("Varlık oluşturulamadı.");
    public static Error AssetNotFound => new("Varlık bulunamadı.");
    public static Error AssetRateNotFound => new("Varlık oranı bulunamadı.");
    public static Error AssetUpdateFailed => new("Varlık güncellenemedi.");
    public static Error AssetDeletionFailed => new("Varlık silinemedi.");
    public static Error InvalidAssetId => new("Geçersiz varlık ID'si.");

}
