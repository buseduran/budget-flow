using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Assets.Queries.GetAssetPagination;
public class GetAssetPaginationQuery : IRequest<Result<PaginatedList<AssetResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int WalletID { get; set; }
    public class GetAssetPaginationQueryHandler : IRequestHandler<GetAssetPaginationQuery, Result<PaginatedList<AssetResponse>>>
    {
        private readonly IAssetRepository assetRepository;
        private readonly IUserWalletRepository userWalletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly ICurrencyRateRepository currencyRateRepository;
        public GetAssetPaginationQueryHandler(IAssetRepository assetRepository, IUserWalletRepository userWalletRepository, IHttpContextAccessor httpContextAccessor, ICurrencyRateRepository currencyRateRepository)
        {
            this.assetRepository = assetRepository;
            this.userWalletRepository = userWalletRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.currencyRateRepository = currencyRateRepository;
        }
        public async Task<Result<PaginatedList<AssetResponse>>> Handle(GetAssetPaginationQuery request, CancellationToken cancellationToken)
        {
            var assetList = await assetRepository.GetAssetsAsync(request.Page, request.PageSize);

            int userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            var userWallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
            if (userWallet == null)
                return Result.Failure<PaginatedList<AssetResponse>>(UserWalletErrors.UserWalletNotFound);

            //convert to usercurrency from try
            var userCurrency = userWallet.Wallet.Currency;
            var currencyRate = await currencyRateRepository.GetCurrencyRateByType(userCurrency);

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
