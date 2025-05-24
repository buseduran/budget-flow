using FluentValidation;

namespace BudgetFlow.Application.Investments.Queries;

public class GetPortfolioAssetsValidator : AbstractValidator<GetPortfolioAssetsQuery>
{
    public GetPortfolioAssetsValidator()
    {
        RuleFor(x => x.Portfolio)
            .NotEmpty().WithMessage("Portföy adı boş olamaz.")
            .MaximumLength(100).WithMessage("Portföy adı 100 karakterden uzun olamaz.");
    }
}