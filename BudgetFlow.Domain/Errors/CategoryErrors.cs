using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class CategoryErrors
{
    public static Error CreationFailed => new("Kategori oluşturulamadı.");
    public static Error NameOrColorCannotBeEmpty => new("Kategori adı ve rengi boş olamaz.");
    public static Error InvalidCategoryId => new("Geçersiz kategori ID'si.");
    public static Error DeletionFailed => new("Kategori silinemedi.");
    public static Error UpdateFailed => new("Kategori güncellenemedi.");
    public static Error CategoryNotFound => new("Kategori bulunamadı.");
    public static Error CannotDeleteCategoryWithEntries => new("Kategori silinemedi, bu kategoriyle ilişkili girdi(ler) bulunmaktadır.");

}
