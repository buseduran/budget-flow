using BudgetFlow.Domain.Shared;

namespace BudgetFlow.Domain.Errors;
public static class PortfolioErrors
{
    public static Error PortfolioNotFound => new("Portfolio.NotFound", "Portföy bulunamadı.");
    public static Error PortfolioAlreadyExists => new("Portfolio.AlreadyExists", "Bu isimde bir portföy zaten var.");
    public static Error PortfolioCreationFailed => new("Portfolio.CreationFailed", "Portföy oluşturulamadı.");
    public static Error PortfolioUpdateFailed => new("Portfolio.UpdateFailed", "Portföy güncellenemedi.");
    public static Error PortfolioDeletionFailed => new("Portfolio.DeletionFailed", "Portföy silinemedi.");
    public static Error PortfolioHasInvestments => new("Portfolio.HasInvestments", "Bu portföye ait yatırım kayıtları bulunmaktadır. Önce yatırımları silmelisiniz.");
}
