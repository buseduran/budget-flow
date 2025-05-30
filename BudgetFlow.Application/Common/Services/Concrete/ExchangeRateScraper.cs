﻿using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
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
            foreach (var rate in latestRates)
            {
                await _currencyRateRepository.AddRatesAsync(new[] { rate }, saveChanges: false);

                #region Save to Asset Table
                var asset = new Asset
                {
                    Name = "Döviz Kuru",
                    Code = "FOREX",
                    Symbol = "FOREX",
                    Unit = "TRY",
                    AssetType = AssetType.Exchange,
                    BuyPrice = rate.ForexBuying,
                    SellPrice = rate.ForexSelling,
                    Description = "Döviz Kuru - TCMB",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var existingAsset = await _assetRepository.GetByCodeAsync(asset.Code);
                if (existingAsset != null)
                {
                    asset.ID = existingAsset.ID;
                    await _assetRepository.UpdateAssetAsync(asset, saveChanges: false);
                }
                else
                {
                    await _assetRepository.CreateAssetAsync(asset, saveChanges: false);
                }
                #endregion
            }

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
