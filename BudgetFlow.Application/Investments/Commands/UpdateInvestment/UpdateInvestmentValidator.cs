using FluentValidation;

namespace BudgetFlow.Application.Investments.Commands.UpdateInvestment;

public class UpdateInvestmentValidator : AbstractValidator<UpdateInvestmentCommand>
{
    public UpdateInvestmentValidator()
    {
        RuleFor(x => x.ID)
            .GreaterThan(0).WithMessage("Geçersiz yatırım ID'si.");

        RuleFor(x => x.UnitAmount)
            .GreaterThan(0).WithMessage("Birim miktar 0'dan büyük olmalıdır.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıklama 500 karakterden uzun olamaz.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Tarih boş olamaz.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Tarih bugünden sonra olamaz.");
    }
}