using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using HtmlAgilityPack;
using System.Globalization;

namespace BudgetFlow.Application.Common.Services.Concrete
{
    public class StockScraper : IStockScraper
    {
        private readonly IAssetRepository assetRepository;

        public StockScraper(IAssetRepository assetRepository)
        {
            this.assetRepository = assetRepository;
        }

        public async Task<IEnumerable<Asset>> GetStocksAsync(AssetType assetType)
        {
            List<Asset> assets = [];
            var cultureInfo = new CultureInfo("tr-TR");

            #region HTML verisi alınır
            HttpClient client = new();
            //var apiUrl = Environment.GetEnvironmentVariable("API_URL") ?? "";
            var apiUrl = "https://uzmanpara.milliyet.com.tr/canli-borsa/";
            var html = await client.GetStringAsync(apiUrl);
            #endregion

            #region Tablo verileri alınır
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(html);
            var nodes = htmlDocument.DocumentNode
                .Descendants("tr")
                .Where(node => node.GetAttributeValue("class", "").Contains("zebra"))
                .ToList();
            #endregion

            #region Veriler modele dönüştürülür
            foreach (var node in nodes)
            {
                var stockName = node.Id.ToString().Split("h_tr_id_").Last();
                var stockPrice = node
                    .Descendants("td")
                    .Where(x => x.GetAttributeValue("id", "").Contains("h_td_fiyat"))
                    .ToList().First().InnerText;
                var stockPercentage = node
                    .Descendants("td")
                    .Where(x => x.GetAttributeValue("id", "").Contains("h_td_yuzde"))
                    .ToList().First().InnerText;
                var date = node
                    .Descendants("td")
                    .Where(x => x.GetAttributeValue("id", "").Contains("h_td_zaman"))
                    .ToList().First().InnerText;

                decimal.TryParse(stockPrice, NumberStyles.Currency, cultureInfo, out var price);
                double.TryParse(stockPercentage, NumberStyles.Any, cultureInfo, out var percentage);

                var existAsset = await assetRepository.GetByCodeAsync(stockName);
                // check if not exist in db
                if(existAsset != null)
                {
                    
                }
                Asset asset = new()
                {
                    Name = stockName,
                    Code = stockName,
                    BuyPrice = price,
                    SellPrice = price,
                    Description = percentage.ToString(),
                    Unit = "",
                    AssetType = assetType,
                };
                assets.Add(asset);
                //var result = await assetRepository.CreateAssetAsync(asset);
                //return result

                //else it will update
                //var asset = await assetRepository.GetAssetByCodeAsync(stockName);
                //if (asset != null)
                //{
                //    asset.BuyPrice = price;
                //    asset.SellPrice = price;
                //    asset.Description = percentage.ToString();
                //    asset.Unit = "";
                //    asset.AssetType = assetType;
                //}
                //var result = await assetRepository.UpdateAssetAsync(asset);

            }
            #endregion

            return assets;
        }
    }
}
