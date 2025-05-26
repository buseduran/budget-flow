using BudgetFlow.Application.Common.Jobs;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using MediatR;

namespace BudgetFlow.Application.Assets.Commands.SyncAsset;
public class SyncAssetCommand : IRequest<Result<bool>>
{
    public AssetType _AssetType { get; set; }
    public class SyncAssetCommandHandler : IRequestHandler<SyncAssetCommand, Result<bool>>
    {
        private readonly IStockScraper _stockScraper;
        private readonly MetalJob _metalJob;
        public SyncAssetCommandHandler(
            IStockScraper stockScraper,
            MetalJob metalJob)
        {
            _stockScraper = stockScraper;
            _metalJob = metalJob;
        }

        public async Task<Result<bool>> Handle(SyncAssetCommand request, CancellationToken cancellationToken)
        {
            if (request._AssetType == AssetType.Stock)
            {
                var stocks = await _stockScraper.GetStocksAsync(request._AssetType);
            }
            if (request._AssetType == AssetType.Metal)
            {
                await _metalJob.ExecuteAsync();
            }

            return Result.Success(true);
        }
    }

}
