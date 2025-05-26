using AutoMapper;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Budget.Commands.UpdateEntry;
public class UpdateEntryCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public EntryDto Entry { get; set; }
    public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand, Result<bool>>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly ICurrencyRateRepository _currencyRateRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateEntryCommandHandler(
            IBudgetRepository budgetRepository,
            ICurrentUserService currentUserService,
            IWalletRepository walletRepository,
            IMapper mapper,
            ICategoryRepository categoryRepository,
            IUserWalletRepository userWalletRepository,
            IUnitOfWork unitOfWork,
            ICurrencyRateRepository currencyRateRepository)
        {
            _budgetRepository = budgetRepository;
            _currentUserService = currentUserService;
            _walletRepository = walletRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _userWalletRepository = userWalletRepository;
            _unitOfWork = unitOfWork;
            _currencyRateRepository = currencyRateRepository;
        }
        public async Task<Result<bool>> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var mappedEntry = _mapper.Map<Entry>(request.Entry);
            mappedEntry.UserID = userID;

            var category = await _categoryRepository.GetCategoryByIdAsync(mappedEntry.CategoryID);

            #region Eski Tutar Üzerinden Farkı Hesapla ve Normalize Et
            var existingEntry = await _budgetRepository.GetEntryByIdAsync(request.ID);
            if (existingEntry == null)
                return Result.Failure<bool>(EntryErrors.EntryNotFound);

            var newAmount = category.Type == EntryType.Income
                ? Math.Abs(mappedEntry.Amount)
                : -Math.Abs(mappedEntry.Amount);

            #region AmountInTRY alanını güncelle
            var userWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.Entry.WalletID, userID);
            decimal exchangeRateToTRY = 1m;
            if (userWallet.Wallet.Currency != CurrencyType.TRY)
            {
                var currencyRate = await _currencyRateRepository.GetCurrencyRateByType(userWallet.Wallet.Currency);
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

            var wallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.Entry.WalletID, userID);
            if (category.Type == EntryType.Expense && wallet.Wallet.Balance < Math.Abs(difference) && difference < 0)
                return Result.Failure<bool>(WalletErrors.InsufficientBalance);
            #endregion

            #region Wallet ve Entry Güncelle
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _walletRepository.UpdateWalletAsync(userID, difference, differenceInTry, saveChanges: false);

                mappedEntry.Amount = newAmount;
                mappedEntry.Currency = existingEntry.Currency;

                await _budgetRepository.UpdateEntryAsync(request.ID, mappedEntry, saveChanges: false);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure<bool>(GeneralErrors.FromMessage(ex.Message));
            }
            #endregion
        }
    }
}
