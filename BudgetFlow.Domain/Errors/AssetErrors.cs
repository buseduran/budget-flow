using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;

public static class AssetErrors
{
    public static Error CreationFailed => new("Asset.CreationFailed", "Varlık oluşturulamadı.");
    public static Error AssetNotFound => new("Asset.NotFound", "Varlık bulunamadı.");
    public static Error AssetRateNotFound => new("Asset.RateNotFound", "Varlık oranı bulunamadı.");
    public static Error AssetUpdateFailed => new("Asset.UpdateFailed", "Varlık güncellenemedi.");
    public static Error AssetDeletionFailed => new("Asset.DeletionFailed", "Varlık silinemedi.");
    public static Error InvalidAssetId => new("Asset.InvalidId", "Geçersiz varlık ID'si.");
}
