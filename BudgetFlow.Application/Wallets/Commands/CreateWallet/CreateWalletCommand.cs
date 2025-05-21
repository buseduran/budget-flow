using BudgetFlow.Application.Common.Interfaces;
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
        private readonly IUserWalletRepository userWalletRepository;
        private readonly ICurrencyRateRepository currencyRateRepository;
        private readonly IUnitOfWork unitOfWork;
        public CreateWalletCommandHandler(
            IWalletRepository walletRepository,
            IBudgetRepository budgetRepository,
            IHttpContextAccessor httpContextAccessor,
            ICategoryRepository categoryRepository,
            IUserWalletRepository userWalletRepository,
            ICurrencyRateRepository currencyRateRepository,
        IUnitOfWork unitOfWork)
        {
            this.walletRepository = walletRepository;
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.categoryRepository = categoryRepository;
            this.userWalletRepository = userWalletRepository;
            this.currencyRateRepository = currencyRateRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
        {
            if (request.Balance < 0)
                return Result.Failure<bool>(WalletErrors.InvalidOpeningBalance);

            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            var hasExistingOwnerWallet = await userWalletRepository.GetUserWalletByRoleAsync(userID, WalletRole.Owner);
            if (hasExistingOwnerWallet is not null)
                return Result.Failure<bool>(WalletErrors.UserWalletAlreadyExists);

            await unitOfWork.BeginTransactionAsync();
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
                await categoryRepository.CreateCategoryAsync(category, saveChanges: false);

                #region ExchangeRate alınır.
                decimal exchangeRateToTRY = 1m;
                var currencyRate = await currencyRateRepository.GetCurrencyRateByType(request.Currency);
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
                await walletRepository.CreateWalletAsync(wallet, saveChanges: false);

                // İlk SaveChanges — Category ve Wallet ID’leri garanti altına alınır
                await unitOfWork.SaveChangesAsync();

                // Kullanıcı-Cüzdan ilişkisi
                var userWallet = new UserWallet
                {
                    UserID = userID,
                    WalletID = wallet.ID,
                    Role = WalletRole.Owner
                };
                var userWalletCreated = await userWalletRepository.CreateAsync(userWallet, saveChanges: false);

                // Entry oluştur
                var entry = new Entry
                {
                    Name = "Başlangıç Bakiyesi",
                    Amount = request.Balance,
                    AmountInTRY = request.Balance * exchangeRateToTRY,
                    Date = DateTime.UtcNow,
                    CategoryID = category.ID,
                    UserID = userID,
                    WalletID = wallet.ID
                };
                var entryResult = await budgetRepository.CreateEntryAsync(entry, saveChanges: false);

                // Final save
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
