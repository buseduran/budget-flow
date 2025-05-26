using FluentValidation;

namespace BudgetFlow.Application.Budget.Commands.CreateEntry;

public class CreateEntryCommandValidator : AbstractValidator<CreateEntryCommand>
{
    public CreateEntryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("İşlem adı boş olamaz")
            .MaximumLength(100).WithMessage("İşlem adı 100 karakterden uzun olamaz");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Tutar boş olamaz")
            .GreaterThan(0).WithMessage("Tutar 0'dan büyük olmalıdır");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Tarih boş olamaz")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Tarih bugünden sonra olamaz");

        RuleFor(x => x.CategoryID)
            .NotEmpty().WithMessage("Kategori seçilmelidir")
            .GreaterThan(0).WithMessage("Geçersiz kategori ID'si");

        RuleFor(x => x.WalletID)
            .NotEmpty().WithMessage("Cüzdan seçilmelidir")
            .GreaterThan(0).WithMessage("Geçersiz cüzdan ID'si");
    }
}