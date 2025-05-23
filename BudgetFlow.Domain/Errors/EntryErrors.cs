using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class EntryErrors
{
    public static Error CreationFailed => new("Entry.CreationFailed", "Girdi oluşturulamadı.");
    public static Error EntryNotFound => new("Entry.NotFound", "Girdi bulunamadı.");
    public static Error EntryUpdateFailed => new("Entry.UpdateFailed", "Girdi güncellenemedi.");
    public static Error EntryDeletionFailed => new("Entry.DeletionFailed", "Girdi silinemedi.");
    public static Error InvalidEntryId => new("Entry.InvalidId", "Geçersiz girdi ID'si.");
    public static Error AnalysisEntriesRetrievalFailed => new("Entry.AnalysisEntriesRetrievalFailed", "Analiz girdileri alınamadı.");
    public static Error LatestEntriesRetrievalFailed => new("Entry.LatestEntriesRetrievalFailed", "Son girdiler alınamadı.");
}
