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
        private readonly MetalJob _metalJob;
        private readonly StockJob _stockJob;
        private readonly CurrencyJob _currencyJob;
        public SyncAssetCommandHandler(
            MetalJob metalJob,
            StockJob stockJob,
            CurrencyJob currencyJob)
        {
            _metalJob = metalJob;
            _stockJob = stockJob;
            _currencyJob = currencyJob;
        }

        public async Task<Result<bool>> Handle(SyncAssetCommand request, CancellationToken cancellationToken)
        {
            await _currencyJob.ExecuteAsync(); //usd, eur, gbp
            await _stockJob.ExecuteAsync(); // midas
            await _metalJob.ExecuteAsync(); // gold, silver
            return Result.Success(true);
        }
    }
}
