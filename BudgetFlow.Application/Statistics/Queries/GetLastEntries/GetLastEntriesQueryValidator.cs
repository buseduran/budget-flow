using FluentValidation;

namespace BudgetFlow.Application.Statistics.Queries.GetLastEntries;

public class GetLastEntriesQueryValidator : AbstractValidator<GetLastEntriesQuery>
{
    public GetLastEntriesQueryValidator()
    {
        RuleFor(x => x.WalletID)
            .NotEmpty().WithMessage("Cüzdan ID'si boş olamaz")
            .GreaterThan(0).WithMessage("Geçersiz cüzdan ID'si");
    }
}