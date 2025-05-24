using FluentValidation;

namespace BudgetFlow.Application.Budget.Commands.UpdateEntry;

public class UpdateEntryCommandValidator : AbstractValidator<UpdateEntryCommand>
{
    public UpdateEntryCommandValidator()
    {
        RuleFor(x => x.ID)
            .NotEmpty().WithMessage("İşlem ID'si boş olamaz")
            .GreaterThan(0).WithMessage("Geçersiz işlem ID'si");

        RuleFor(x => x.Entry.Name)
            .NotEmpty().WithMessage("İşlem adı boş olamaz")
            .MaximumLength(100).WithMessage("İşlem adı 100 karakterden uzun olamaz");

        RuleFor(x => x.Entry.Amount)
            .NotEmpty().WithMessage("Tutar boş olamaz")
            .GreaterThan(0).WithMessage("Tutar 0'dan büyük olmalıdır");

        RuleFor(x => x.Entry.Date)
            .NotEmpty().WithMessage("Tarih boş olamaz")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Tarih bugünden sonra olamaz");

        RuleFor(x => x.Entry.CategoryID)
            .NotEmpty().WithMessage("Kategori seçilmelidir")
            .GreaterThan(0).WithMessage("Geçersiz kategori ID'si");

        RuleFor(x => x.Entry.WalletID)
            .NotEmpty().WithMessage("Cüzdan seçilmelidir")
            .GreaterThan(0).WithMessage("Geçersiz cüzdan ID'si");
    }
}