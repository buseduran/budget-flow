using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using HtmlAgilityPack;
using System.Globalization;

namespace BudgetFlow.Infrastructure.Services;
public class MetalScraper : IMetalScraper
{
    private readonly HttpClient _httpClient;

    public MetalScraper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Asset>> GetMetalsAsync(AssetType assetType)
    {
        var url = "https://bigpara.hurriyet.com.tr/altin/";
        var html = await _httpClient.GetStringAsync(url);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var now = DateTime.Now;
        var assets = new List<Asset>();

        var rows = htmlDoc.DocumentNode.SelectNodes("//div[@class='tBody']/ul");

        if (rows is null)
            return Enumerable.Empty<Asset>();

        foreach (var row in rows)
        {
            var cells = row.SelectNodes("li");
            if (cells == null || cells.Count < 3)
                continue;

            var name = HtmlEntity.DeEntitize(cells[0].InnerText.Trim());
            var buyText = cells[1].InnerText.Trim().Replace(".", "").Replace(",", ".");
            var sellText = cells[2].InnerText.Trim().Replace(".", "").Replace(",", ".");

            if (!decimal.TryParse(buyText, NumberStyles.Any, CultureInfo.InvariantCulture, out var buyPrice) ||
                !decimal.TryParse(sellText, NumberStyles.Any, CultureInfo.InvariantCulture, out var sellPrice))
                continue;

            var (code, symbol, unit) = MapAltinType(name);

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

        return assets;
    }


    private (string Code, string Symbol, string Unit) MapAltinType(string name)
    {
        name = name.ToLowerInvariant();

        if (name.Contains("çeyrek"))
            return ("XAU-Q", "CAlt", "adet");
        if (name.Contains("yarım"))
            return ("XAU-H", "YAlt", "adet");
        if (name.Contains("tam"))
            return ("XAU-F", "TAlt", "adet");
        if (name.Contains("gram"))
            return ("XAU-G", "GAlt", "gram");
        if (name.Contains("ons"))
            return ("XAU-O", "OAlt", "ons");

        // fallback
        return ("XAU-UNK", "Altin", "birim");
    }

}
