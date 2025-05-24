using AutoMapper;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Commands.UpdateEntry;
public class UpdateEntryCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public EntryDto Entry { get; set; }
    public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand, Result<bool>>
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IWalletRepository walletRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;
        private readonly IUserWalletRepository userWalletRepository;
        private readonly ICurrencyRateRepository currencyRateRepository;
        private readonly IUnitOfWork unitOfWork;
        public UpdateEntryCommandHandler(
            IBudgetRepository budgetRepository,
            IHttpContextAccessor httpContextAccessor,
            IWalletRepository walletRepository,
            IMapper mapper,
            ICategoryRepository categoryRepository,
            IUserWalletRepository userWalletRepository,
            IUnitOfWork unitOfWork,
            ICurrencyRateRepository currencyRateRepository)
        {
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.walletRepository = walletRepository;
            this.mapper = mapper;
            this.categoryRepository = categoryRepository;
            this.userWalletRepository = userWalletRepository;
            this.unitOfWork = unitOfWork;
            this.currencyRateRepository = currencyRateRepository;
        }
        public async Task<Result<bool>> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var mappedEntry = mapper.Map<Entry>(request.Entry);
            mappedEntry.UserID = userID;

            var category = await categoryRepository.GetCategoryByIdAsync(mappedEntry.CategoryID);

            #region Eski Tutar Üzerinden Farkı Hesapla ve Normalize Et
            var existingEntry = await budgetRepository.GetEntryByIdAsync(request.ID);
            if (existingEntry == null)
                return Result.Failure<bool>(EntryErrors.EntryNotFound);

            var newAmount = category.Type == EntryType.Income
                ? Math.Abs(mappedEntry.Amount)
                : -Math.Abs(mappedEntry.Amount);

            #region AmountInTRY alanını güncelle
            var userWallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(request.Entry.WalletID, userID);
            decimal exchangeRateToTRY = 1m;
            if (userWallet.Wallet.Currency != CurrencyType.TRY)
            {
                var currencyRate = await currencyRateRepository.GetCurrencyRateByType(userWallet.Wallet.Currency);
                if (currencyRate is null)
                    return Result.Failure<bool>(CurrencyRateErrors.FetchFailed);
                exchangeRateToTRY = currencyRate.ForexSelling;
            }
            mappedEntry.AmountInTRY = category.Type == EntryType.Income
                             ? Math.Abs(mappedEntry.Amount * exchangeRateToTRY)
                             : -Math.Abs(mappedEntry.Amount * exchangeRateToTRY);
            mappedEntry.ExchangeRate = exchangeRateToTRY;
            mappedEntry.Currency = userWallet.Wallet.Currency;
            #endregion

            var difference = newAmount - existingEntry.Amount;
            var differenceInTry = category.Type == EntryType.Income
                ? Math.Abs(difference * exchangeRateToTRY)
                : -Math.Abs(difference * exchangeRateToTRY);

            var wallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(request.Entry.WalletID, userID);
            if (category.Type == EntryType.Expense && wallet.Wallet.Balance < Math.Abs(difference) && difference < 0)
                return Result.Failure<bool>(WalletErrors.InsufficientBalance);
            #endregion

            #region Wallet ve Entry Güncelle
            await unitOfWork.BeginTransactionAsync();
            try
            {
                await walletRepository.UpdateWalletAsync(userID, difference, differenceInTry, saveChanges: false);

                mappedEntry.Amount = newAmount;
                mappedEntry.Currency = existingEntry.Currency;

                await budgetRepository.UpdateEntryAsync(request.ID, mappedEntry, saveChanges: false);

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                return Result.Failure<bool>(GeneralErrors.FromMessage(ex.Message));
            }
            #endregion
        }
    }
}
