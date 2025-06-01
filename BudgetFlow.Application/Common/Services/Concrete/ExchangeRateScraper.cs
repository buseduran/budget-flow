using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;

namespace BudgetFlow.Application.Common.Services.Concrete;
public class ExchangeRateScraper : IExchangeRateScraper
{
    private readonly ICurrencyRateRepository _currencyRateRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly HttpClient _httpClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string TcmbUrl;
    public ExchangeRateScraper(
        ICurrencyRateRepository currencyRateRepository,
        IAssetRepository assetRepository,
        HttpClient httpClient,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _currencyRateRepository = currencyRateRepository;
        _assetRepository = assetRepository;
        _httpClient = httpClient;
        _unitOfWork = unitOfWork;
        TcmbUrl = configuration["Tcmb:ExchangeRateUrl"];
    }

    public async Task<IEnumerable<CurrencyRate>> GetExchangeRatesAsync()
    {
        try
        {
            var xml = await _httpClient.GetStringAsync(TcmbUrl);
            var doc = XDocument.Parse(xml);

            var currencies = doc.Descendants("Currency")
                .Where(x =>
                    x.Attribute("Kod")?.Value is "USD" or "EUR" or "GBP")
                .Select(x =>
                {
                    var code = x.Attribute("Kod")?.Value!;
                    return new CurrencyRate
                    {
                        CurrencyType = Enum.Parse<CurrencyType>(code),
                        ForexBuying = decimal.TryParse(x.Element("ForexBuying")?.Value, out var buy) ? buy : 0,
                        ForexSelling = decimal.TryParse(x.Element("ForexSelling")?.Value, out var sell) ? sell : 0,
                        RetrievedAt = DateTime.UtcNow
                    };
                })
                .ToList();

            return currencies;
        }
        catch (Exception)
        {
            return Enumerable.Empty<CurrencyRate>();
        }
    }

    public async Task<Result<bool>> SyncExchangeRatesAsync()
    {
        var latestRates = await GetExchangeRatesAsync();

        if (!latestRates.Any())
            return Result.Failure<bool>(CurrencyRateErrors.FetchFailed);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _currencyRateRepository.AddRatesAsync(latestRates, saveChanges: false);

            var assetsDbSet = _unitOfWork.GetAssets();
            var existingAssets = await assetsDbSet
                .Where(a => a.AssetType == AssetType.Exchange)
                .ToDictionaryAsync(a => a.Code);

            var now = DateTime.UtcNow;
            var assetsToUpdate = new List<Asset>();
            var assetsToInsert = new List<Asset>();

            #region Prepare Stock Assets for Bulk Update/Insert 
            foreach (var rate in latestRates)
            {
                var asset = new Asset
                {
                    Name = rate.CurrencyType.ToString(),
                    Code = rate.CurrencyType.ToString(),
                    Symbol = rate.CurrencyType.ToString(),
                    Unit = "adet",
                    AssetType = AssetType.Exchange,
                    BuyPrice = rate.ForexBuying,
                    SellPrice = rate.ForexSelling,
                    Description = $"{rate.CurrencyType.ToString()} Döviz Kuru - TCMB",
                    CreatedAt = now,
                    UpdatedAt = now
                };

                if (existingAssets.TryGetValue(asset.Code, out var existingAsset))
                {
                    existingAsset.BuyPrice = asset.BuyPrice;
                    existingAsset.SellPrice = asset.SellPrice;
                    existingAsset.Description = asset.Description;
                    existingAsset.UpdatedAt = now;
                    assetsToUpdate.Add(existingAsset);
                }
                else
                {
                    assetsToInsert.Add(asset);
                }
            }
            #endregion

            #region Bulk Update/Insert Assets
            if (assetsToUpdate.Any()) assetsDbSet.UpdateRange(assetsToUpdate);
            if (assetsToInsert.Any()) await assetsDbSet.AddRangeAsync(assetsToInsert);
            #endregion

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Result.Success(true);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            return Result.Failure<bool>(CurrencyRateErrors.CreationFailed);
        }
    }
}
