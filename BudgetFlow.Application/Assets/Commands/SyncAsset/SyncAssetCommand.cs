using BudgetFlow.Application.Common.Jobs;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;

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
            #region Asset Senkronizasyonu
            await _currencyJob.ExecuteAsync(); // usd, eur, gbp
            await _stockJob.ExecuteAsync();    // midas
            await _metalJob.ExecuteAsync();    // gold, silver
            #endregion


            var allAssets = await _assetRepository.GetAllAsync();
            var assetDict = allAssets.ToDictionary(a => a.ID, a => a);

            var walletAssets = await _walletAssetRepository.GetAllAsync();

            var walletDeltaDict = new Dictionary<int, decimal>();

            foreach (var walletAsset in walletAssets)
            {
                if (assetDict.TryGetValue(walletAsset.AssetId, out var asset))
                {
                    var oldBalance = walletAsset.Balance;
                    var newBalance = asset.BuyPrice * walletAsset.Amount;

                    var delta = newBalance - oldBalance;
                    walletAsset.Balance = newBalance;

                    if (!walletDeltaDict.ContainsKey(walletAsset.WalletId))
                        walletDeltaDict[walletAsset.WalletId] = 0;

                    walletDeltaDict[walletAsset.WalletId] += delta;

                    await _walletAssetRepository.UpdateAsync(walletAsset);
                }
            }

            #region Cüzdanları getir ve bakiyelerini güncelle
            var wallets = await _walletRepository.GetAllAsync();
            foreach (var wallet in wallets)
            {
                if (walletDeltaDict.TryGetValue(wallet.ID, out var delta))
                {
                    wallet.Balance += delta;
                    await _walletRepository.UpdateAsync(wallet);
                }
            }
            #endregion


            return Result.Success(true);
        }

        private static MetalType? GetMetalType(string name)
        {
            name = name.ToLowerInvariant();

            var metalTypes = new Dictionary<string, MetalType>
            {
                ["çeyrek altın"] = MetalType.GoldQuarter,
                ["yarım altın"] = MetalType.GoldHalf,
                ["tam altın"] = MetalType.GoldFull,
                ["altin (tl/gr)"] = MetalType.GoldGram,
                ["altın (ons)"] = MetalType.GoldOunce,
                ["gümüş (tl/gr)"] = MetalType.SilverGram,
                ["gümüş ($/ons)"] = MetalType.SilverOunce
            };

            return metalTypes.FirstOrDefault(x => name.Contains(x.Key)).Value;
        }
    }
}