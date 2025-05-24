using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class CategoryErrors
{
    public static Error CreationFailed => new("Category.CreationFailed", "Kategori oluşturulamadı.");
    public static Error NameOrColorCannotBeEmpty => new("Category.NameOrColorCannotBeEmpty", "Kategori adı ve rengi boş olamaz.");
    public static Error InvalidCategoryId => new("Category.InvalidId", "Geçersiz kategori ID'si.");
    public static Error DeletionFailed => new("Category.DeletionFailed", "Kategori silinemedi.");
    public static Error UpdateFailed => new("Category.UpdateFailed", "Kategori güncellenemedi.");
    public static Error CategoryNotFound => new("Category.NotFound", "Kategori bulunamadı.");
    public static Error CannotDeleteCategoryWithEntries => new("Category.CannotDeleteWithEntries", "Kategori silinemedi, bu kategoriyle ilişkili girdi(ler) bulunmaktadır.");
}
