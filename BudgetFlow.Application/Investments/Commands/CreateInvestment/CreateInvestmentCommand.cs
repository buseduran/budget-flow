using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Investments.Commands.CreateInvestment;
public class CreateInvestmentCommand : IRequest<Result<bool>>
{
    public int AssetId { get; set; }
    public decimal UnitAmount { get; set; }
    public string Description { get; set; }
    public InvestmentType Type { get; set; }
    public DateTime Date { get; set; }
    public int PortfolioId { get; set; }
    public class CreateInvestmentCommandHandler : IRequestHandler<CreateInvestmentCommand, Result<bool>>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInvestmentCommandHandler(
            IInvestmentRepository investmentRepository,
            IWalletRepository walletRepository,
            IUserWalletRepository userWalletRepository,
            IAssetRepository assetRepository,
            IPortfolioRepository portfolioRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _investmentRepository = investmentRepository;
            _walletRepository = walletRepository;
            _userWalletRepository = userWalletRepository;
            _assetRepository = assetRepository;
            _portfolioRepository = portfolioRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(CreateInvestmentCommand request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var portfolio = await _portfolioRepository.GetPortfolioByIdAsync(request.PortfolioId);
            if (portfolio is null)
                return Result.Failure<bool>(PortfolioErrors.PortfolioNotFound);

            var wallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(portfolio.WalletID, userID);
            if (wallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);

            var asset = await _assetRepository.GetAssetAsync(request.AssetId);

            var investment = new Investment
            {
                AssetId = request.AssetId,
                UserId = userID,
                UnitAmount = request.UnitAmount,
                Description = request.Description,
                Date = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc),
                PortfolioId = request.PortfolioId,
                Type = request.Type
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
                if (walletAsset is not null)
                {
                    if (investment.Type == InvestmentType.Buy)
                    {
                        walletAsset.Amount += investment.UnitAmount;
                        walletAsset.Balance = walletAsset.Amount * asset.SellPrice;

                        var walletAssetUpdate = await _walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance, saveChanges: false);
                        var walletUpdate = await _walletRepository.UpdateWalletAsync(userID, -investment.CurrencyAmount, saveChanges: false);
                    }
                    else
                    {
                        if (walletAsset.Amount < investment.UnitAmount)
                            return Result.Failure<bool>(WalletAssetErrors.NotEnoughAssetAmount);

                        walletAsset.Amount -= investment.UnitAmount;
                        walletAsset.Balance = walletAsset.Amount * asset.BuyPrice;

                        var walletAssetUpdate = await _walletRepository.UpdateWalletAssetAsync(walletAsset.ID, walletAsset.Amount, walletAsset.Balance, saveChanges: false);
                        var walletUpdate = await _walletRepository.UpdateWalletAsync(userID, investment.CurrencyAmount, saveChanges: false);
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
                            Balance = investment.CurrencyAmount
                        }, saveChanges: false);

                        var walletUpdate = await _walletRepository.UpdateWalletAsync(userID, -investment.CurrencyAmount, saveChanges: false);
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