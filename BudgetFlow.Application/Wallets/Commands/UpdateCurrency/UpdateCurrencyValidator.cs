using FluentValidation;

namespace BudgetFlow.Application.Wallets.Commands.UpdateCurrency;

public class UpdateCurrencyValidator : AbstractValidator<UpdateCurrencyCommand>
{
    public UpdateCurrencyValidator()
    {
        RuleFor(x => x.WalletID)
            .GreaterThan(0).WithMessage("Geçersiz cüzdan ID'si.");

        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Geçersiz para birimi.");
    }
}