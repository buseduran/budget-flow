using FluentValidation;

namespace BudgetFlow.Application.Categories.Commands.CreateCategory;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı boş olamaz.")
            .MaximumLength(100).WithMessage("Kategori adı 100 karakterden uzun olamaz.");

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Kategori rengi boş olamaz.")
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$").WithMessage("Geçersiz renk formatı. Hex formatında olmalıdır (örn: #FF0000).");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Geçersiz kategori tipi.");
    }
}