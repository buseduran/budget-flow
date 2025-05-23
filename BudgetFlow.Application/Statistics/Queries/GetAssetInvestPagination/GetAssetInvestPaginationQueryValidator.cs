using FluentValidation;

namespace BudgetFlow.Application.Statistics.Queries.GetAssetInvestPagination;

public class GetAssetInvestPaginationQueryValidator : AbstractValidator<GetAssetInvestPaginationQuery>
{
    public GetAssetInvestPaginationQueryValidator()
    {
        RuleFor(x => x.PortfolioID)
            .NotEmpty().WithMessage("Portföy ID'si boş olamaz")
            .GreaterThan(0).WithMessage("Geçersiz portföy ID'si");

        RuleFor(x => x.AssetID)
            .NotEmpty().WithMessage("Varlık ID'si boş olamaz")
            .GreaterThan(0).WithMessage("Geçersiz varlık ID'si");

        RuleFor(x => x.WalletID)
            .NotEmpty().WithMessage("Cüzdan ID'si boş olamaz")
            .GreaterThan(0).WithMessage("Geçersiz cüzdan ID'si");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(100).WithMessage("Sayfa boyutu 100'den büyük olamaz");
    }
}