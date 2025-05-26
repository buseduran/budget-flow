using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Wallets.Commands.CreateWallet;
public class CreateWalletCommand : IRequest<Result<bool>>
{
    public decimal Balance { get; set; }
    public CurrencyType Currency { get; set; }

    public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, Result<bool>>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly ICurrencyRateRepository _currencyRateRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateWalletCommandHandler(
            IWalletRepository walletRepository,
            IBudgetRepository budgetRepository,
            ICurrentUserService currentUserService,
            ICategoryRepository categoryRepository,
            IUserWalletRepository userWalletRepository,
            ICurrencyRateRepository currencyRateRepository,
        IUnitOfWork unitOfWork)
        {
            _walletRepository = walletRepository;
            _budgetRepository = budgetRepository;
            _currentUserService = currentUserService;
            _categoryRepository = categoryRepository;
            _userWalletRepository = userWalletRepository;
            _currencyRateRepository = currencyRateRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
        {
            if (request.Balance < 0)
                return Result.Failure<bool>(WalletErrors.InvalidOpeningBalance);

            var userID = _currentUserService.GetCurrentUserID();

            var hasExistingOwnerWallet = await _userWalletRepository.GetUserWalletByRoleAsync(userID, WalletRole.Owner);
            if (hasExistingOwnerWallet is not null)
                return Result.Failure<bool>(WalletErrors.UserWalletAlreadyExists);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Kategori oluştur
                var category = new Category
                {
                    Name = "Başlangıç Bakiyesi",
                    Color = "#000000",
                    Type = EntryType.OpeningBalance,
                    UserID = userID
                };
                await _categoryRepository.CreateCategoryAsync(category, saveChanges: false);

                #region ExchangeRate alınır.
                if (!Enum.IsDefined(typeof(CurrencyType), request.Currency))
                {
                    return Result.Failure<bool>(WalletErrors.InvalidCurrency);
                }

                var currencyRate = await _currencyRateRepository.GetCurrencyRateByType(request.Currency);
                if (currencyRate == null)
                    return Result.Failure<bool>(WalletErrors.CurrencyRateNotFound);

                decimal exchangeRateToTRY = 1m;

                if (request.Currency != CurrencyType.TRY)
                {
                    exchangeRateToTRY = currencyRate.ForexSelling;
                }
                #endregion

                // Cüzdan oluştur
                var wallet = new Wallet
                {
                    Balance = request.Balance,
                    BalanceInTRY = request.Balance * exchangeRateToTRY,
                    Currency = request.Currency,
                };
                await _walletRepository.CreateWalletAsync(wallet, saveChanges: false);

                // İlk SaveChanges — Category ve Wallet ID'leri garanti altına alınır
                await _unitOfWork.SaveChangesAsync();

                // Kullanıcı-Cüzdan ilişkisi
                var userWallet = new UserWallet
                {
                    UserID = userID,
                    WalletID = wallet.ID,
                    Role = WalletRole.Owner
                };
                var userWalletCreated = await _userWalletRepository.CreateAsync(userWallet, saveChanges: false);

                // Entry oluştur
                var entry = new Entry
                {
                    Name = "Başlangıç Bakiyesi",
                    Amount = request.Balance,
                    AmountInTRY = request.Balance * exchangeRateToTRY,
                    Date = DateTime.UtcNow,
                    CategoryID = category.ID,
                    UserID = userID,
                    WalletID = wallet.ID,
                    Currency = request.Currency
                };
                var entryResult = await _budgetRepository.CreateEntryAsync(entry, saveChanges: false);

                // Final save
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure<bool>(GeneralErrors.FromMessage(ex.Message));
            }
        }
    }
}
