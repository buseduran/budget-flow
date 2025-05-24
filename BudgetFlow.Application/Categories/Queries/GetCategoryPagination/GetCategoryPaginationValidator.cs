using FluentValidation;

namespace BudgetFlow.Application.Categories.Queries.GetCategoryPagination;

public class GetCategoryPaginationValidator : AbstractValidator<GetCategoryPaginationQuery>
{
    public GetCategoryPaginationValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Sayfa boyutu 100'den büyük olamaz.");
    }
}