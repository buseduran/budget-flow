using FluentValidation;

namespace BudgetFlow.Application.Budget.Commands.DeleteEntry;

public class DeleteEntryCommandValidator : AbstractValidator<DeleteEntryCommand>
{
    public DeleteEntryCommandValidator()
    {
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage("İşlem ID'si boş olamaz")
            .GreaterThan(0).WithMessage("Geçersiz işlem ID'si");
    }
}