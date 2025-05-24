using FluentValidation;

namespace BudgetFlow.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.ID)
            .GreaterThan(0).WithMessage("Geçersiz kategori ID'si.");

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Kategori rengi boş olamaz.")
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$").WithMessage("Geçersiz renk formatı. Hex formatında olmalıdır (örn: #FF0000).");
    }
}