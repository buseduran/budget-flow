using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class PortfolioErrors
{
    public static Error PortfolioNotFound => new("Portföy bulunamadı.");
    public static Error PortfolioAlreadyExists => new("Bu isimde bir portföy zaten var.");
    public static Error PortfolioCreationFailed => new("Portföy oluşturulamadı.");
    public static Error PortfolioUpdateFailed => new("Portföy güncellenemedi.");
    public static Error PortfolioDeletionFailed => new("Portföy silinemedi.");
}
