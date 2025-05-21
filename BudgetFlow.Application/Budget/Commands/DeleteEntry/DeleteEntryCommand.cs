using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Commands.DeleteEntry;
public class DeleteEntryCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public DeleteEntryCommand(int ID)
    {
        this.ID = ID;
    }

    public class DeleteEntryCommandHandler : IRequestHandler<DeleteEntryCommand, Result<bool>>
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IUserWalletRepository userWalletRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICurrencyRateRepository currencyRateRepository;
        private readonly IUnitOfWork unitOfWork;

        public DeleteEntryCommandHandler(
            IBudgetRepository budgetRepository,
            IWalletRepository walletRepository,
            IUserWalletRepository userWalletRepository,
            ICategoryRepository categoryRepository,
            IHttpContextAccessor httpContextAccessor,
            ICurrencyRateRepository currencyRateRepository,
            IUnitOfWork unitOfWork)
        {
            this.budgetRepository = budgetRepository;
            this.walletRepository = walletRepository;
            this.userWalletRepository = userWalletRepository;
            this.categoryRepository = categoryRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.currencyRateRepository = currencyRateRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            var existingEntry = await budgetRepository.GetEntryByIdAsync(request.ID);
            if (existingEntry == null)
                return Result.Failure<bool>(EntryErrors.EntryNotFound);

            var category = await categoryRepository.GetCategoryByIdAsync(existingEntry.Category.ID);
            if (category == null)
                return Result.Failure<bool>(CategoryErrors.CategoryNotFound);

            var wallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(existingEntry.WalletID, userID);
            if (wallet == null)
                return Result.Failure<bool>(EntryErrors.EntryNotFound); // veya WalletErrors.WalletNotFound

            decimal balanceAdjustment = category.Type == EntryType.Income
                ? -Math.Abs(existingEntry.Amount)
                : Math.Abs(existingEntry.Amount);

            var currency= wallet.Wallet.Currency;
            decimal exchangeRateToTRY = 1m;
            var currencyRate = await currencyRateRepository.GetCurrencyRateByType(currency);
            if (currency != CurrencyType.TRY)
            {
                exchangeRateToTRY = currencyRate.ForexSelling;
            }

            decimal balanceAdjustmentInTRY = category.Type == EntryType.Income
                ? -Math.Abs(existingEntry.Amount) * exchangeRateToTRY
                : Math.Abs(existingEntry.Amount) * exchangeRateToTRY;

            await unitOfWork.BeginTransactionAsync();
            try
            {
                var walletUpdateResult = await walletRepository.UpdateWalletAsync(userID, balanceAdjustment, balanceAdjustmentInTRY, saveChanges: false);

                var deleteResult = await budgetRepository.DeleteEntryAsync(request.ID, saveChanges: false);

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                return Result.Failure<bool>(GeneralErrors.FromMessage(ex.Message));
            }
        }
    }
}
