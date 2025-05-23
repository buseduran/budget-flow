using FluentValidation;

namespace BudgetFlow.Application.Statistics.Queries.GetAssetRevenue;

public class GetAssetRevenueQueryValidator : AbstractValidator<GetAssetRevenueQuery>
{
    public GetAssetRevenueQueryValidator()
    {
        RuleFor(x => x.Portfolio)
            .NotEmpty().WithMessage("Portföy adı boş olamaz")
            .MaximumLength(100).WithMessage("Portföy adı 100 karakterden uzun olamaz");
    }
}