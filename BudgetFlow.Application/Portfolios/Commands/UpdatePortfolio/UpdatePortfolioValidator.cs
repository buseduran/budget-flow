using FluentValidation;

namespace BudgetFlow.Application.Portfolios.Commands.UpdatePortfolio;

public class UpdatePortfolioValidator : AbstractValidator<UpdatePortfolioCommand>
{
    public UpdatePortfolioValidator()
    {
        RuleFor(x => x.ID)
            .GreaterThan(0).WithMessage("Geçersiz portföy ID'si.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Portföy adı boş olamaz.")
            .MaximumLength(100).WithMessage("Portföy adı 100 karakterden uzun olamaz.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Portföy açıklaması 500 karakterden uzun olamaz.");
    }
}