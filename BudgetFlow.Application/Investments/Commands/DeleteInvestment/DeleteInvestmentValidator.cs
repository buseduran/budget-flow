using FluentValidation;

namespace BudgetFlow.Application.Investments.Commands.DeleteInvestment;

public class DeleteInvestmentValidator : AbstractValidator<DeleteInvestmentCommand>
{
    public DeleteInvestmentValidator()
    {
        RuleFor(x => x.ID)
            .GreaterThan(0).WithMessage("Geçersiz yatırım ID'si.");
    }
}