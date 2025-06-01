using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using HtmlAgilityPack;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using BudgetFlow.Application.Common.Scrapers.Abstract;

namespace BudgetFlow.Application.Common.Scrapers.Concrete;

public class MetalScraper : IMetalScraper
{
    private readonly HttpClient _httpClient;
    private readonly ICurrencyRateRepository _currencyRateRepository;
    private readonly string _bigParaUrl;
    private static readonly Dictionary<MetalType, (string Code, string Symbol, string Unit)> MetalTypes = new()
    {
        { MetalType.GoldQuarter, ("XAU-Q", "CAlt", "adet") },
        { MetalType.GoldHalf, ("XAU-H", "YAlt", "adet") },
        { MetalType.GoldFull, ("XAU-F", "TAlt", "adet") },
        { MetalType.GoldGram, ("XAU-G", "GAlt", "gram") },
        { MetalType.GoldOunce, ("XAU-O", "OAlt", "ons") },
        { MetalType.SilverGram, ("XAG-G", "GAg", "gram") },
        { MetalType.SilverOunce, ("XAG-O", "OAg", "ons") }
    };

    public MetalScraper(HttpClient httpClient, ICurrencyRateRepository currencyRateRepository, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _currencyRateRepository = currencyRateRepository;
        _bigParaUrl = configuration["ScraperUrls:BigPara"];
    }

    public async Task<IEnumerable<Asset>> GetMetalsAsync(AssetType assetType)
    {
        var html = await _httpClient.GetStringAsync(_bigParaUrl);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var now = DateTime.Now;
        var assets = new List<Asset>();

        // Get USD/TRY exchange rate
        var usdRate = await _currencyRateRepository.GetCurrencyRateByType(CurrencyType.USD);
        if (usdRate == null)
            return Enumerable.Empty<Asset>();

        #region Table
        var goldTable = htmlDoc.DocumentNode.SelectNodes("//div[@class='tBody']/ul");
        if (goldTable is null)
            return Enumerable.Empty<Asset>();

        foreach (var row in goldTable)
        {
            var cells = row.SelectNodes("li");
            if (cells == null || cells.Count < 3)
                continue;

            var name = HtmlEntity.DeEntitize(cells[0].InnerText.Trim());
            var metalType = GetMetalType(name);
            if (metalType == null)
                continue;

            var buyText = cells[1].InnerText.Trim().Replace(".", "").Replace(",", ".");
            var sellText = cells[2].InnerText.Trim().Replace(".", "").Replace(",", ".");

            if (!decimal.TryParse(buyText, NumberStyles.Any, CultureInfo.InvariantCulture, out var buyPrice) ||
                !decimal.TryParse(sellText, NumberStyles.Any, CultureInfo.InvariantCulture, out var sellPrice))
                continue;

            var (code, symbol, unit) = MetalTypes[metalType.Value];

            // Convert USD to TRY for GoldOunce and SilverOunce
            if (metalType == MetalType.GoldOunce || metalType == MetalType.SilverOunce)
            {
                buyPrice *= usdRate.ForexSelling;
                sellPrice *= usdRate.ForexSelling;
            }

            assets.Add(new Asset
            {
                Name = name,
                Code = code,
                Symbol = symbol,
                Unit = unit,
                AssetType = assetType,
                BuyPrice = buyPrice,
                SellPrice = sellPrice,
                CreatedAt = now
            });
        }
        #endregion

        return assets;
    }

    private static MetalType? GetMetalType(string name)
    {
        name = name.ToLowerInvariant();

        if (name.Contains("çeyrek altın"))
            return MetalType.GoldQuarter;
        if (name.Contains("yarım altın"))
            return MetalType.GoldHalf;
        if (name.Contains("tam altın"))
            return MetalType.GoldFull;
        if (name.Contains("altın (tl/gr)"))
            return MetalType.GoldGram;
        if (name.Contains("altın (ons)"))
            return MetalType.GoldOunce;
        if (name.Contains("gümüş (tl/gr)"))
            return MetalType.SilverGram;
        if (name.Contains("gümüş ($/ons)"))
            return MetalType.SilverOunce;

        return null;
    }
}
