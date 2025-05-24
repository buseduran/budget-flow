using FluentValidation;

namespace BudgetFlow.Application.Portfolios.Queries.GetPortfolio;

public class GetPortfolioValidator : AbstractValidator<GetPortfolioQuery>
{
    public GetPortfolioValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Portföy adı boş olamaz.")
            .MaximumLength(100).WithMessage("Portföy adı 100 karakterden uzun olamaz.");
    }
}