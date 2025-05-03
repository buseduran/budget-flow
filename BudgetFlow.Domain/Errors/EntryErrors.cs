using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class EntryErrors
{
    public static Error CreationFailed => new("Girdi oluşturulamadı.");
    public static Error EntryNotFound => new("Girdi bulunamadı.");
    public static Error EntryUpdateFailed => new("Girdi güncellenemedi.");
    public static Error EntryDeletionFailed => new("Girdi silinemedi.");
    public static Error InvalidEntryId => new("Geçersiz girdi ID'si.");
    public static Error AnalysisEntriesRetrievalFailed => new("Analiz girdileri alınamadı.");
    public static Error LatestEntriesRetrievalFailed => new("Son girdiler alınamadı.");
}
