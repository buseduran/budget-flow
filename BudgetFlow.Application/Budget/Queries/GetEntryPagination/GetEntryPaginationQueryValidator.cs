using FluentValidation;

namespace BudgetFlow.Application.Budget.Queries.GetEntryPagination;

public class GetEntryPaginationQueryValidator : AbstractValidator<GetEntryPaginationQuery>
{
    public GetEntryPaginationQueryValidator()
    {
        RuleFor(x => x.Page)
            .NotEmpty().WithMessage("Sayfa numarası boş olamaz")
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır");

        RuleFor(x => x.PageSize)
            .NotEmpty().WithMessage("Sayfa boyutu boş olamaz")
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(100).WithMessage("Sayfa boyutu 100'den büyük olamaz");

        RuleFor(x => x.WalletID)
            .NotEmpty().WithMessage("Cüzdan ID'si boş olamaz")
            .GreaterThan(0).WithMessage("Geçersiz cüzdan ID'si");
    }
}