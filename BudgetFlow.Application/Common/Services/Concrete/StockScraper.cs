using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace BudgetFlow.Application.Common.Services.Concrete
{
    public class StockScraper : IStockScraper
    {
        private readonly HttpClient _httpClient;
        private readonly string _midasApiUrl;

        public StockScraper(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _midasApiUrl = configuration["ScraperUrls:Midas"];

            // Headers'ı elle ekle
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/115.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
            _httpClient.DefaultRequestHeaders.Add("Referer", "https://www.getmidas.com/");
            _httpClient.DefaultRequestHeaders.Add("Origin", "https://www.getmidas.com");
        }

        public async Task<IEnumerable<Asset>> GetStocksAsync(AssetType assetType)
        {
            var response = await _httpClient.GetStringAsync(_midasApiUrl);
            var jsonArrayString = JsonSerializer.Deserialize<string>(response);

            var midasItems = JsonSerializer.Deserialize<List<MidasItem>>(jsonArrayString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var assets = midasItems.Select(item => new Asset
            {
                Name = item.Code,
                AssetType = assetType,
                Code = item.Code,
                BuyPrice = item.Bid,
                SellPrice = item.Ask,
                Description = $"Change: {item.DailyChange} ({item.DailyChangePercent}%)",
                Unit = "adet"
            }).ToList();

            return assets;
        }
    }
}
