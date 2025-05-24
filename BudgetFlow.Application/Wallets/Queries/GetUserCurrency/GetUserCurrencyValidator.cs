using FluentValidation;

namespace BudgetFlow.Application.Wallets.Queries.GetUserCurrency;

public class GetUserCurrencyValidator : AbstractValidator<GetUserCurrencyQuery>
{
    public GetUserCurrencyValidator()
    {
        RuleFor(x => x.WalletID)
            .GreaterThan(0).WithMessage("Geçersiz cüzdan ID'si.");
    }
}