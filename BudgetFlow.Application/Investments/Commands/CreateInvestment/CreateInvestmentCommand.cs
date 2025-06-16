using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using BudgetFlow.Application.Categories;
using MediatR;

namespace BudgetFlow.Application.Investments.Commands.CreateInvestment;
public class CreateInvestmentCommand : IRequest<Result<bool>>
{
    public int AssetID { get; set; }
    public decimal UnitAmount { get; set; }
    public string Description { get; set; }
    public InvestmentType Type { get; set; }
    public DateTime Date { get; set; }
    public int PortfolioID { get; set; }
    public bool TrackOnly { get; set; } = false;
    public class CreateInvestmentCommandHandler : IRequestHandler<CreateInvestmentCommand, Result<bool>>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBudgetRepository _budgetRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CreateInvestmentCommandHandler(
            IInvestmentRepository investmentRepository,
            IWalletRepository walletRepository,
            IUserWalletRepository userWalletRepository,
            IAssetRepository assetRepository,
            IPortfolioRepository portfolioRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            IBudgetRepository budgetRepository,
            ICategoryRepository categoryRepository)
        {
            _investmentRepository = investmentRepository;
            _walletRepository = walletRepository;
            _userWalletRepository = userWalletRepository;
            _assetRepository = assetRepository;
            _portfolioRepository = portfolioRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _budgetRepository = budgetRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<bool>> Handle(CreateInvestmentCommand request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var portfolio = await _portfolioRepository.GetPortfolioByIdAsync(request.PortfolioID);
            if (portfolio is null)
                return Result.Failure<bool>(PortfolioErrors.PortfolioNotFound);

            var wallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(portfolio.WalletID, userID);
            if (wallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);

            var asset = await _assetRepository.GetAssetAsync(request.AssetID);

            if (asset.AssetType == AssetType.Stock && request.UnitAmount != Math.Floor(request.UnitAmount))
            {
                return Result.Failure<bool>(InvestmentErrors.InvalidStockAmount);
            }

            var investment = new Investment
            {
                AssetId = request.AssetID,
                UserId = userID,
                UnitAmount = request.UnitAmount,
                Description = request.Description,
                Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
                PortfolioId = request.PortfolioID,
                Type = request.Type,
                TrackOnly = request.TrackOnly
            };

            investment.CurrencyAmount = investment.Type == InvestmentType.Buy
                ? investment.UnitAmount * asset.SellPrice
                : investment.UnitAmount * asset.BuyPrice;

            investment.ExchangeRate = investment.Type == InvestmentType.Buy
                ? asset.SellPrice : asset.BuyPrice;

            var walletAsset = await _walletRepository.GetWalletAssetAsync(portfolio.WalletID, investment.AssetId);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Get or create investment categories
                var investmentCategory = await _categoryRepository.GetCategoriesAsync(1, 1, portfolio.WalletID);
                var categoryName = request.Type == InvestmentType.Buy ? "Yatırım Alış" : "Yatırım Satış";
                var category = investmentCategory.Items.FirstOrDefault(c => c.Name == categoryName);

                if (category == null)
                {
                    var newCategory = new Category
                    {
                        Name = categoryName,
                        Color = request.Type == InvestmentType.Buy ? "#F44336" : "#4CAF50", // Red for buy (expense), Green for sell (income)
                        Type = request.Type == InvestmentType.Buy ? EntryType.Expense : EntryType.Income,
                        WalletID = portfolio.WalletID
                    };
                    var categoryId = await _categoryRepository.CreateCategoryAsync(newCategory, saveChanges: true);
                    await _unitOfWork.SaveChangesAsync();
                    category = new CategoryResponse
                    {
                        ID = categoryId,
                        Name = categoryName,
                        Color = request.Type == InvestmentType.Buy ? "#F44336" : "#4CAF50",
                        Type = request.Type == InvestmentType.Buy ? EntryType.Expense : EntryType.Income
                    };
                }

                if (walletAsset is not null)
                {
                    if (investment.Type == InvestmentType.Buy)
                    {
                        walletAsset.Amount += investment.UnitAmount;
                        walletAsset.Balance += investment.UnitAmount * asset.BuyPrice;

                        var walletAssetUpdate = await _walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance, saveChanges: false);

                        if (!investment.TrackOnly)
                        {
                            var walletUpdate = await _walletRepository.UpdateWalletAsync(portfolio.WalletID, -investment.CurrencyAmount, saveChanges: false);

                            // Create entry for investment purchase
                            var entry = new Entry
                            {
                                Name = $"{asset.Name} yatırımı",
                                Amount = -investment.CurrencyAmount, // Negative because it's an expense
                                Date = investment.Date,
                                CategoryID = category.ID,
                                WalletID = portfolio.WalletID,
                                UserID = userID
                            };
                            await _budgetRepository.CreateEntryAsync(entry, saveChanges: false);
                        }
                        else
                        {
                            // For TrackOnly investments, increase wallet balance as it's a savings
                            var walletUpdate = await _walletRepository.UpdateWalletAsync(portfolio.WalletID, investment.UnitAmount * asset.BuyPrice, saveChanges: false);
                        }
                    }
                    else
                    {
                        if (walletAsset.Amount < investment.UnitAmount)
                            return Result.Failure<bool>(WalletAssetErrors.NotEnoughAssetAmount);

                        walletAsset.Amount -= investment.UnitAmount;
                        walletAsset.Balance -= investment.UnitAmount * asset.BuyPrice;

                        var walletAssetUpdate = await _walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance, saveChanges: false);

                        if (!investment.TrackOnly)
                        {
                            var walletUpdate = await _walletRepository.UpdateWalletAsync(portfolio.WalletID, investment.CurrencyAmount, saveChanges: false);

                            // Create entry for investment sale
                            var entry = new Entry
                            {
                                Name = $"{asset.Name} satışı",
                                Amount = investment.CurrencyAmount, // Positive because it's income from sale
                                Date = investment.Date,
                                CategoryID = category.ID,
                                WalletID = portfolio.WalletID,
                                UserID = userID
                            };
                            await _budgetRepository.CreateEntryAsync(entry, saveChanges: false);
                        }
                        else
                        {
                            // For TrackOnly investments, decrease wallet balance as it's a withdrawal from savings
                            var walletUpdate = await _walletRepository.UpdateWalletAsync(portfolio.WalletID, -investment.UnitAmount * asset.SellPrice, saveChanges: false);
                        }
                    }
                }
                else
                {
                    if (investment.Type == InvestmentType.Buy)
                    {
                        var createResult = await _walletRepository.CreateWalletAssetAsync(new WalletAsset
                        {
                            WalletId = portfolio.WalletID,
                            AssetId = investment.AssetId,
                            Amount = investment.UnitAmount,
                            Balance = investment.UnitAmount * asset.BuyPrice
                        }, saveChanges: false);

                        if (!investment.TrackOnly)
                        {
                            var walletUpdate = await _walletRepository.UpdateWalletAsync(portfolio.WalletID, -investment.CurrencyAmount, saveChanges: false);

                            // Create entry for initial investment purchase
                            var entry = new Entry
                            {
                                Name = $"{asset.Name} ilk yatırımı",
                                Amount = -investment.CurrencyAmount, // Negative because it's an expense
                                Date = investment.Date,
                                CategoryID = category.ID,
                                WalletID = portfolio.WalletID,
                                UserID = userID
                            };
                            await _budgetRepository.CreateEntryAsync(entry, saveChanges: false);
                        }
                        else
                        {
                            // For TrackOnly investments, increase wallet balance as it's a savings
                            var walletUpdate = await _walletRepository.UpdateWalletAsync(portfolio.WalletID, investment.UnitAmount * asset.BuyPrice, saveChanges: false);
                        }
                    }
                    else
                    {
                        return Result.Failure<bool>(WalletErrors.NoBalanceForAsset);
                    }
                }

                var investmentResult = await _investmentRepository.CreateInvestmentAsync(investment, saveChanges: false);

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