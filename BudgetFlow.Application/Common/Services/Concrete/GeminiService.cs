using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Services.Abstract;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace BudgetFlow.Application.Common.Services.Concrete;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;
    private readonly string _basePrompt;

    public GeminiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiUrl = config["Gemini:ApiUrl"];
        _basePrompt = @"
            Aşağıdaki gelir/gider verilerini profesyonelce analiz et. Yalnızca son kullanıcının görüntüleyebileceği JSON verisi hakkında herhangi bir bilgi verme ve teknik açıklamalar yapma.
            Gelir ve gider dağılımlarını değerlendir, tasarruf oranına dair çıkarım yap ve 1-2 kısa öneri ver. 
            Maksimum 5-6 cümlelik kısa ama net bir yorum yap. Emojilerle destekle.
            Veriler:
            
            JSON:
            {0}
            
            Analiz:";
    }

    public async Task<string> GenerateDailyAnalysisAsync(DailyFinanceData data)
    {
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        var prompt = string.Format(_basePrompt, jsonData);

        return await GenerateAnalysisAsync(prompt);
    }

    public async Task<string> GenerateBudgetAnalysisAsync(BudgetAnalysisRequest request)
    {
        var jsonData = JsonConvert.SerializeObject(request.BudgetData, Formatting.Indented);
        var dateRange = $"{request.StartDate:dd MMMM yyyy} - {request.EndDate:dd MMMM yyyy}";
        var prompt = string.Format(_basePrompt, jsonData)
            .Replace("bütçe özetidir", $"{dateRange} tarihleri arasındaki {request.AnalysisType} bütçe özetidir");

        return await GenerateAnalysisAsync(prompt);
    }

    private async Task<string> GenerateAnalysisAsync(string prompt)
    {
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl);
        request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        dynamic result = JsonConvert.DeserializeObject(responseContent);
        return result?.candidates?[0]?.content?.parts?[0]?.text ?? "Analiz alınamadı.";
    }
}