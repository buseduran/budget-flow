using FluentValidation;

namespace BudgetFlow.Application.Wallets.Commands.CreateWallet;

public class CreateWalletValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletValidator()
    {
        RuleFor(x => x.Balance)
            .GreaterThanOrEqualTo(0).WithMessage("Bakiye 0'dan küçük olamaz.");

        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Geçersiz para birimi.");
    }
}