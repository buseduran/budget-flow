using BudgetFlow.Application.Common.Jobs;
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
        private readonly IStockScraper stockScraper;
        private readonly MetalJob metalJob;
        public SyncAssetCommandHandler(
            IStockScraper stockScraper,
            MetalJob metalJob)
        {
            this.stockScraper = stockScraper;
            this.metalJob = metalJob;
        }

        public async Task<Result<bool>> Handle(SyncAssetCommand request, CancellationToken cancellationToken)
        {
            if (request.AssetType == AssetType.Stock)
            {
                var stocks = await stockScraper.GetStocksAsync(request.AssetType);
            }
            if (request.AssetType == AssetType.Metal)
            {
                await metalJob.ExecuteAsync();
            }




            return Result.Success(true);
        }
    }

}
