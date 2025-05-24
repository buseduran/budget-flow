using AutoMapper;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

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
        private readonly IBudgetRepository budgetRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IUserWalletRepository userWalletRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IWalletAuthService walletAuthService;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrencyRateRepository currencyRateRepository;
        public CreateEntryCommandHandler(
            IBudgetRepository budgetRepository,
            IWalletRepository walletRepository,
            IUserWalletRepository userWalletRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IWalletAuthService walletAuthService,
            IUnitOfWork unitOfWork,
            ICurrencyRateRepository currencyRateRepository)
        {
            this.budgetRepository = budgetRepository;
            this.walletRepository = walletRepository;
            this.userWalletRepository = userWalletRepository;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.walletAuthService = walletAuthService;
            this.unitOfWork = unitOfWork;
            this.currencyRateRepository = currencyRateRepository;
        }

        public async Task<Result<bool>> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            #region Check user has a wallet and user's role for this wallet
            var wallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
            if (wallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);

            await walletAuthService.EnsureUserHasAccessAsync(request.WalletID, userID, WalletRole.Owner);
            #endregion

            var entry = new EntryDto
            {
                Name = request.Name,
                Amount = request.Amount,
                Date = request.Date,
                CategoryID = request.CategoryID,
                WalletID = request.WalletID,
            };

            var mappedEntry = mapper.Map<Entry>(entry);
            mappedEntry.UserID = userID;

            var category = await categoryRepository.GetCategoryByIdAsync(mappedEntry.CategoryID);
            if (category is null)
                return Result.Failure<bool>(CategoryErrors.CategoryNotFound);

            #region Check user's wallet balance is enough
            if ((category.Type == EntryType.Expense) && (wallet.Wallet.Balance < Math.Abs(mappedEntry.Amount)))
                return Result.Failure<bool>(WalletErrors.InsufficientBalance);
            #endregion

            #region Create entry and Update wallet
            await unitOfWork.BeginTransactionAsync();
            try
            {
                //userın currency'sini al ve ata
                var currency = wallet.Wallet.Currency;
                mappedEntry.Currency = currency;

                #region TRY ile işlem yapılmıyorsa dönüşüm yapılır.
                decimal exchangeRateToTRY = 1m;
                var currencyRate = await currencyRateRepository.GetCurrencyRateByType(currency);
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
                await budgetRepository.CreateEntryAsync(mappedEntry, saveChanges: false);

                // Cüzdan güncelle
                await walletRepository.UpdateWalletAsync(wallet.WalletID, mappedEntry.Amount, mappedEntry.AmountInTRY, saveChanges: false);

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
