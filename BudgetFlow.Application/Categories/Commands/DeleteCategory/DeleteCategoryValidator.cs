using FluentValidation;

namespace BudgetFlow.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator()
    {
        RuleFor(x => x.ID)
            .GreaterThan(0).WithMessage("Geçersiz kategori ID'si.");
    }
}