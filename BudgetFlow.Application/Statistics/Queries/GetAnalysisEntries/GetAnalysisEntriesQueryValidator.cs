using FluentValidation;

namespace BudgetFlow.Application.Statistics.Queries.GetAnalysisEntries;

public class GetAnalysisEntriesQueryValidator : AbstractValidator<GetAnalysisEntriesQuery>
{
    public GetAnalysisEntriesQueryValidator()
    {
        RuleFor(x => x.WalletID)
            .NotEmpty().WithMessage("Cüzdan ID'si boş olamaz")
            .GreaterThan(0).WithMessage("Geçersiz cüzdan ID'si");

        RuleFor(x => x.Range)
            .NotEmpty().WithMessage("Tarih aralığı boş olamaz")
            .MaximumLength(50).WithMessage("Tarih aralığı 50 karakterden uzun olamaz");
    }
}