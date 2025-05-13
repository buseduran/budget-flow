using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Wallets.Commands.CreateWallet;
public class CreateWalletCommand : IRequest<Result<bool>>
{
    public decimal Balance { get; set; }
    public CurrencyType Currency { get; set; }

    public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, Result<bool>>
    {
        private readonly IWalletRepository walletRepository;
        private readonly IBudgetRepository budgetRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public CreateWalletCommandHandler(IWalletRepository walletRepository, IBudgetRepository budgetRepository, IHttpContextAccessor httpContextAccessor, ICategoryRepository categoryRepository)
        {
            this.walletRepository = walletRepository;
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.categoryRepository = categoryRepository;
        }
        public async Task<Result<bool>> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
        {
            if (request.Balance <= 0)
                return Result.Failure<bool>(WalletErrors.InvalidOpeningBalance);

            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            #region Başlangıç bakiyesi ilk Entry olarak kaydedilir
            var category = new Category
            {
                Name = "Başlangıç Bakiyesi",
                Color = "#000000",
                Type = EntryType.OpeningBalance,
                UserID = userID
            };
            var categoryResult = await categoryRepository.CreateCategoryAsync(category);
            if (categoryResult is 0)
                return Result.Failure<bool>(CategoryErrors.CreationFailed);

            var entry = new Entry
            {
                Name = "Başlangıç Bakiyesi",
                Amount = request.Balance,
                Date = DateTime.UtcNow,
                CategoryID = categoryResult,
                UserID = userID,
            };
            var entryResult = await budgetRepository.CreateEntryAsync(entry);
            if (!entryResult)
                return Result.Failure<bool>(EntryErrors.CreationFailed);
            #endregion

            var wallet = new Wallet
            {
                Balance = request.Balance,
                Currency = request.Currency,
                //UserId= userID
            };
            var result = await walletRepository.CreateWalletAsync(wallet);
            return result
                ? Result.Success(true)
                : Result.Failure<bool>(WalletErrors.CreationFailed);
        }
    }
}
