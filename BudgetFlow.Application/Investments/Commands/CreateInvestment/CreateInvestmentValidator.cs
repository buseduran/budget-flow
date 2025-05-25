using FluentValidation;

namespace BudgetFlow.Application.Investments.Commands.CreateInvestment;

public class CreateInvestmentValidator : AbstractValidator<CreateInvestmentCommand>
{
    public CreateInvestmentValidator()
    {
        RuleFor(x => x)
            .NotNull().WithMessage("Yatırım bilgileri boş olamaz.");

        When(x => x != null, () =>
        {
            RuleFor(x => x.AssetId)
                .GreaterThan(0).WithMessage("Geçersiz varlık ID'si.");

            RuleFor(x => x.UnitAmount)
                .GreaterThan(0).WithMessage("Birim miktar 0'dan büyük olmalıdır.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama 500 karakterden uzun olamaz.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Tarih boş olamaz.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Tarih bugünden sonra olamaz.");

            RuleFor(x => x.PortfolioId)
                .GreaterThan(0).WithMessage("Geçersiz portföy ID'si.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Geçersiz yatırım tipi.");
        });
    }
}