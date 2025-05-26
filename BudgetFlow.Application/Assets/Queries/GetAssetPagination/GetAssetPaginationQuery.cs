using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Assets.Queries.GetAssetPagination;
public class GetAssetPaginationQuery : IRequest<Result<PaginatedList<AssetResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int WalletID { get; set; }
    public class GetAssetPaginationQueryHandler : IRequestHandler<GetAssetPaginationQuery, Result<PaginatedList<AssetResponse>>>
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrencyRateRepository _currencyRateRepository;
        public GetAssetPaginationQueryHandler(IAssetRepository assetRepository, IUserWalletRepository userWalletRepository, ICurrentUserService currentUserService, ICurrencyRateRepository currencyRateRepository)
        {
            _assetRepository = assetRepository;
            _userWalletRepository = userWalletRepository;
            _currentUserService = currentUserService;
            _currencyRateRepository = currencyRateRepository;
        }
        public async Task<Result<PaginatedList<AssetResponse>>> Handle(GetAssetPaginationQuery request, CancellationToken cancellationToken)
        {
            var assetList = await _assetRepository.GetAssetsAsync(request.Page, request.PageSize);

            int userID = _currentUserService.GetCurrentUserID();

            var userWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
            if (userWallet == null)
                return Result.Failure<PaginatedList<AssetResponse>>(UserWalletErrors.UserWalletNotFound);

            //convert to usercurrency from try      
            var userCurrency = userWallet.Wallet.Currency;
            var currencyRate = await _currencyRateRepository.GetCurrencyRateByType(userCurrency);

            if (assetList == null)
                return Result.Failure<PaginatedList<AssetResponse>>(AssetErrors.AssetNotFound);

            // Convert prices from TRY to user's currency
            var convertedAssets = assetList.Items;
            if (userCurrency is not CurrencyType.TRY)
            {
                convertedAssets = assetList.Items.Select(asset => new AssetResponse
                {
                    ID = asset.ID,
                    Name = asset.Name,
                    Code = asset.Code,
                    Symbol = asset.Symbol,
                    Unit = asset.Unit,
                    AssetType = asset.AssetType,
                    BuyPrice = asset.BuyPrice / currencyRate.ForexSelling,
                    SellPrice = asset.SellPrice / currencyRate.ForexSelling,
                    Description = asset.Description
                }).ToList();
            }

            var convertedPaginatedList = new PaginatedList<AssetResponse>(
                convertedAssets,
                assetList.TotalCount,
                assetList.PageNumber,
                assetList.PageSize
            );

            return Result.Success(convertedPaginatedList);
        }
    }
}
