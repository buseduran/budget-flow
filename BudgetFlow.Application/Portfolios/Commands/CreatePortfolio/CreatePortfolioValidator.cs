using FluentValidation;

namespace BudgetFlow.Application.Portfolios.Commands.CreatePortfolio;

public class CreatePortfolioValidator : AbstractValidator<CreatePortfolioCommand>
{
    public CreatePortfolioValidator()
    {
        RuleFor(x => x.Portfolio)
            .NotNull().WithMessage("Portföy bilgileri boş olamaz.");

        RuleFor(x => x.Portfolio.Name)
            .NotEmpty().WithMessage("Portföy adı boş olamaz.")
            .MaximumLength(100).WithMessage("Portföy adı 100 karakterden uzun olamaz.");

        RuleFor(x => x.Portfolio.Description)
            .MaximumLength(500).WithMessage("Portföy açıklaması 500 karakterden uzun olamaz.");

        RuleFor(x => x.Portfolio.WalletID)
            .GreaterThan(0).WithMessage("Geçersiz cüzdan ID'si.");
    }
}