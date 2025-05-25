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
        private readonly IMetalScraper metalScraper;
        public SyncAssetCommandHandler(IAssetRepository assetRepository, IStockScraper stockScraper, IMetalScraper metalScraper)
        {
            this.assetRepository = assetRepository;
            this.stockScraper = stockScraper;
            this.metalScraper = metalScraper;
        }

        public async Task<Result<bool>> Handle(SyncAssetCommand request, CancellationToken cancellationToken)
        {
            if (request.AssetType == AssetType.Stock)
            {
                var stocks = await stockScraper.GetStocksAsync(request.AssetType);
            }
            if (request.AssetType == AssetType.Metal)
            {
                var metals = await metalScraper.GetMetalsAsync(request.AssetType);
            }


            return Result.Success(true);
        }
    }

}
