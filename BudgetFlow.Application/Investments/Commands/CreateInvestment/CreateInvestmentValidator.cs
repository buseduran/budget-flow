using FluentValidation;

namespace BudgetFlow.Application.Investments.Commands.CreateInvestment;

public class CreateInvestmentValidator : AbstractValidator<CreateInvestmentCommand>
{
    public CreateInvestmentValidator()
    {
        RuleFor(x => x.Investment)
            .NotNull().WithMessage("Yatırım bilgileri boş olamaz.");

        When(x => x.Investment != null, () =>
        {
            RuleFor(x => x.Investment.AssetId)
                .GreaterThan(0).WithMessage("Geçersiz varlık ID'si.");

            RuleFor(x => x.Investment.UnitAmount)
                .GreaterThan(0).WithMessage("Birim miktar 0'dan büyük olmalıdır.");

            RuleFor(x => x.Investment.Description)
                .MaximumLength(500).WithMessage("Açıklama 500 karakterden uzun olamaz.");

            RuleFor(x => x.Investment.Date)
                .NotEmpty().WithMessage("Tarih boş olamaz.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Tarih bugünden sonra olamaz.");

            RuleFor(x => x.Investment.PortfolioId)
                .GreaterThan(0).WithMessage("Geçersiz portföy ID'si.");

            RuleFor(x => x.Investment.Type)
                .IsInEnum().WithMessage("Geçersiz yatırım tipi.");
        });
    }
}