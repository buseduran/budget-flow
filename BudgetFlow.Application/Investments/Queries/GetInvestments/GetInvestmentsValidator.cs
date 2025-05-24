using FluentValidation;

namespace BudgetFlow.Application.Investments.Queries.GetInvestments;

public class GetInvestmentsValidator : AbstractValidator<GetInvestmentsQuery>
{
    public GetInvestmentsValidator()
    {
        RuleFor(x => x.PortfolioID)
            .GreaterThan(0).WithMessage("Geçersiz portföy ID'si.");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Sayfa boyutu 100'den büyük olamaz.");
    }
}