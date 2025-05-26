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

namespace BudgetFlow.Application.Budget.Commands.CreateEntry;
public class CreateEntryCommand : IRequest<Result<bool>>
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int CategoryID { get; set; }
    public int WalletID { get; set; }
    public class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, Result<bool>>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWalletAuthService _walletAuthService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrencyRateRepository _currencyRateRepository;
        public CreateEntryCommandHandler(
            IBudgetRepository budgetRepository,
            IWalletRepository walletRepository,
            IUserWalletRepository userWalletRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            ICurrentUserService currentUserService,
            IWalletAuthService walletAuthService,
            IUnitOfWork unitOfWork,
            ICurrencyRateRepository currencyRateRepository)
        {
            _budgetRepository = budgetRepository;
            _walletRepository = walletRepository;
            _userWalletRepository = userWalletRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _walletAuthService = walletAuthService;
            _unitOfWork = unitOfWork;
            _currencyRateRepository = currencyRateRepository;
        }

        public async Task<Result<bool>> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();

            #region Check user has a wallet and user's role for this wallet
            var wallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
            if (wallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);

            await _walletAuthService.EnsureUserHasAccessAsync(request.WalletID, userID, WalletRole.Owner);
            #endregion

            var entry = new EntryDto
            {
                Name = request.Name,
                Amount = request.Amount,
                Date = request.Date,
                CategoryID = request.CategoryID,
                WalletID = request.WalletID,
            };

            var mappedEntry = _mapper.Map<Entry>(entry);
            mappedEntry.UserID = userID;

            var category = await _categoryRepository.GetCategoryByIdAsync(mappedEntry.CategoryID);
            if (category is null)
                return Result.Failure<bool>(CategoryErrors.CategoryNotFound);

            #region Check user's wallet balance is enough
            if ((category.Type == EntryType.Expense) && (wallet.Wallet.Balance < Math.Abs(mappedEntry.Amount)))
                return Result.Failure<bool>(WalletErrors.InsufficientBalance);
            #endregion

            #region Create entry and Update wallet
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                //userın currency'sini al ve ata
                var currency = wallet.Wallet.Currency;
                mappedEntry.Currency = currency;

                #region TRY ile işlem yapılmıyorsa dönüşüm yapılır.
                decimal exchangeRateToTRY = 1m;
                var currencyRate = await _currencyRateRepository.GetCurrencyRateByType(currency);
                if (currencyRate is null)
                    return Result.Failure<bool>(CurrencyRateErrors.FetchFailed);

                if (currency != CurrencyType.TRY)
                {
                    exchangeRateToTRY = currencyRate.ForexSelling;
                }
                mappedEntry.AmountInTRY = mappedEntry.Amount * exchangeRateToTRY;
                mappedEntry.ExchangeRate = exchangeRateToTRY;
                #endregion

                // Miktarı kategori tipine göre ayarla
                mappedEntry.Amount = category.Type == EntryType.Income
                    ? Math.Abs(mappedEntry.Amount)
                    : -Math.Abs(mappedEntry.Amount);
                mappedEntry.AmountInTRY = category.Type == EntryType.Income
                   ? Math.Abs(mappedEntry.AmountInTRY)
                   : -Math.Abs(mappedEntry.AmountInTRY);

                // Entry ekle
                await _budgetRepository.CreateEntryAsync(mappedEntry, saveChanges: false);

                // Cüzdan güncelle
                await _walletRepository.UpdateWalletAsync(wallet.WalletID, mappedEntry.Amount, mappedEntry.AmountInTRY, saveChanges: false);

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
