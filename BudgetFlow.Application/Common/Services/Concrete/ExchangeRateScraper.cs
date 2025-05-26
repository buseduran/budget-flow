using BudgetFlow.Application.Common.Interfaces;
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
    private readonly HttpClient _httpClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string TcmbUrl;
    public ExchangeRateScraper(
        ICurrencyRateRepository currencyRateRepository,
        HttpClient httpClient,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _currencyRateRepository = currencyRateRepository;
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
            foreach (var rate in latestRates)
            {
                var existingRate = await _currencyRateRepository.GetCurrencyRateByType(rate.CurrencyType);
                if (existingRate != null)
                {
                    // Update existing rate
                    existingRate.ForexBuying = rate.ForexBuying;
                    existingRate.ForexSelling = rate.ForexSelling;
                    existingRate.RetrievedAt = DateTime.UtcNow;
                    await _currencyRateRepository.AddRatesAsync(new[] { existingRate }, saveChanges: false);
                }
                else
                {
                    // Create new rate
                    await _currencyRateRepository.AddRatesAsync(new[] { rate }, saveChanges: false);
                }
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
