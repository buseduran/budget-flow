using FluentValidation;

namespace BudgetFlow.Application.Portfolios.Commands.DeletePortfolio;

public class DeletePortfolioValidator : AbstractValidator<DeletePortfolioCommand>
{
    public DeletePortfolioValidator()
    {
        RuleFor(x => x.ID)
            .GreaterThan(0).WithMessage("Geçersiz portföy ID'si.");
    }
}