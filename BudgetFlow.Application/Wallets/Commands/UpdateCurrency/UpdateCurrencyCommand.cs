using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Wallets.Commands.UpdateCurrency;
public class UpdateCurrencyCommand : IRequest<Result<bool>>
{
    public CurrencyType Currency { get; set; }
    public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, Result<bool>>
    {
        private readonly IWalletRepository walletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public UpdateCurrencyCommandHandler(IWalletRepository walletRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.walletRepository = walletRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<bool>> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var result = await walletRepository.UpdateCurrencyAsync(userID, request.Currency);

            return result ? Result.Success(true)
                : Result.Failure<bool>(WalletErrors.UpdateFailed);
        }
    }
}
