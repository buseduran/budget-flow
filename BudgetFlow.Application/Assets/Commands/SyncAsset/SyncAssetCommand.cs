using BudgetFlow.Application.Common.Jobs;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Assets.Commands.SyncAsset;
public class SyncAssetCommand : IRequest<Result<bool>>
{
    public class SyncAssetCommandHandler : IRequestHandler<SyncAssetCommand, Result<bool>>
    {
        private readonly MetalJob _metalJob;
        private readonly StockJob _stockJob;
        private readonly CurrencyJob _currencyJob;
        private readonly IAssetRepository _assetRepository;
        private readonly IWalletAssetRepository _walletAssetRepository;
        private readonly IWalletRepository _walletRepository;
        public SyncAssetCommandHandler(
            MetalJob metalJob,
            StockJob stockJob,
            CurrencyJob currencyJob,
            IAssetRepository assetRepository,
            IWalletAssetRepository walletAssetRepository,
            IWalletRepository walletRepository)
        {
            _metalJob = metalJob;
            _stockJob = stockJob;
            _currencyJob = currencyJob;
            _assetRepository = assetRepository;
            _walletAssetRepository = walletAssetRepository;
            _walletRepository = walletRepository;
        }

        public async Task<Result<bool>> Handle(SyncAssetCommand request, CancellationToken cancellationToken)
        {

            await _currencyJob.ExecuteAsync(); //usd, eur, gbp
            await _stockJob.ExecuteAsync(); // midas
            await _metalJob.ExecuteAsync(); // gold, silver

            // Update wallet assets and wallet balances
            var walletAssets = await _walletAssetRepository.GetAllAsync();
            foreach (var walletAsset in walletAssets)
            {
                var asset = await _assetRepository.GetByIdAsync(walletAsset.AssetId);
                if (asset != null)
                {
                    walletAsset.Balance = asset.SellPrice * walletAsset.Amount;
                    await _walletAssetRepository.UpdateAsync(walletAsset);
                }
            }

            // Update wallet balances
            var wallets = await _walletRepository.GetAllAsync();
            foreach (var wallet in wallets)
            {
                var walletAssetsForWallet = walletAssets.Where(wa => wa.WalletId == wallet.ID);
                wallet.Balance = walletAssetsForWallet.Sum(wa => wa.Balance);
                await _walletRepository.UpdateAsync(wallet);
            }


            return Result.Success(true);
        }
    }
}
