using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using MediatR;

namespace BudgetFlow.Application.Assets.Commands.SyncAsset;
public class SyncAssetCommand : IRequest<Result<bool>>
{
    public AssetType AssetType { get; set; }
    public class SyncAssetCommandHandler : IRequestHandler<SyncAssetCommand, Result<bool>>
    {
        private readonly IAssetRepository assetRepository;
        private readonly IStockScraper stockScraper;
        public SyncAssetCommandHandler(IAssetRepository assetRepository, IStockScraper stockScraper)
        {
            this.assetRepository = assetRepository;
            this.stockScraper = stockScraper;
        }

        public async Task<Result<bool>> Handle(SyncAssetCommand request, CancellationToken cancellationToken)
        {
            #region Stocks (Hisseler) senkronizasyonu
            if (request.AssetType == AssetType.Stock)
            {
                var stocks = await stockScraper.GetStocksAsync(request.AssetType);
            }
            //Asset asset = new()
            //{
            //    Name = request.Asset.Name,
            //    AssetTypeId = request.Asset.AssetTypeId,
            //    BuyPrice = request.Asset.BuyPrice,
            //    SellPrice = request.Asset.SellPrice,
            //    Description = request.Asset.Description,
            //    Symbol = image,
            //    Code = request.Asset.Code,
            //    Unit = request.Asset.Unit
            //};
            //var result = await assetRepository.CreateAssetAsync(asset);
            //return result
            //    ? Result.Success(true)
            //    : Result.Failure<bool>("Failed to create Asset");


            #endregion



            //string image = string.Empty;
            //if (request.Symbol != null && request.Symbol.Length > 0)
            //{
            //    using var memoryStream = new MemoryStream();
            //    await request.Symbol.CopyToAsync(memoryStream, cancellationToken);
            //    image = Convert.ToBase64String(memoryStream.ToArray());
            //}

            //Asset asset = new()
            //{
            //    Name = request.Asset.Name,
            //    AssetTypeId = request.Asset.AssetTypeId,
            //    BuyPrice = request.Asset.BuyPrice,
            //    SellPrice = request.Asset.SellPrice,
            //    Description = request.Asset.Description,
            //    Symbol = image,
            //    Code = request.Asset.Code,
            //    Unit = request.Asset.Unit
            //};
            //var result = await assetRepository.CreateAssetAsync(asset);
            //return result
            //    ? Result.Success(true)
            //    : Result.Failure<bool>("Failed to create Asset");

            return Result.Success(true);
        }
    }

}
