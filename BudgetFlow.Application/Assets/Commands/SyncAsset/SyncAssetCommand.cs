using BudgetFlow.Application.Common.Jobs;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using MediatR;

namespace BudgetFlow.Application.Assets.Commands.SyncAsset;
public class SyncAssetCommand : IRequest<Result<bool>>
{
    public AssetType assetType { get; set; }
    public class SyncAssetCommandHandler : IRequestHandler<SyncAssetCommand, Result<bool>>
    {
        private readonly IStockScraper _stockScraper;
        private readonly MetalJob _metalJob;
        private readonly StockJob _stockJob;
        public SyncAssetCommandHandler(
            IStockScraper stockScraper,
            MetalJob metalJob,
            StockJob stockJob)
        {
            _stockScraper = stockScraper;
            _metalJob = metalJob;
            _stockJob = stockJob;
        }

        public async Task<Result<bool>> Handle(SyncAssetCommand request, CancellationToken cancellationToken)
        {
            //if (request.assetType == AssetType.Stock)
            //{
            await _stockJob.ExecuteAsync();
            //}
            //if (request.assetType == AssetType.Metal)
            //{
            await _metalJob.ExecuteAsync();
            //}

            return Result.Success(true);
        }
    }
}
