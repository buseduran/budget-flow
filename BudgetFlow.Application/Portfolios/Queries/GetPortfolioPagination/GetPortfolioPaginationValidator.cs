using FluentValidation;

namespace BudgetFlow.Application.Portfolios.Queries.GetPortfolioPagination;

public class GetPortfolioPaginationValidator : AbstractValidator<GetPortfolioPaginationQuery>
{
    public GetPortfolioPaginationValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Sayfa boyutu 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Sayfa boyutu 100'den büyük olamaz.");

        RuleFor(x => x.WalletID)
            .GreaterThan(0).WithMessage("Geçersiz cüzdan ID'si.");
    }
}