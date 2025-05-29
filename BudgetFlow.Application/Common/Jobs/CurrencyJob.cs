using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using Quartz;

namespace BudgetFlow.Application.Common.Jobs;
public class CurrencyJob : IJob
{
    private readonly IExchangeRateScraper _exchangeRateScraper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private const string CacheKey = "CurrencyData";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1); // 1 saat

    public CurrencyJob(IExchangeRateScraper exchangeRateScraper, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _exchangeRateScraper = exchangeRateScraper;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await ExecuteAsync();
    }

    public async Task ExecuteAsync()
    {
        var result = await _exchangeRateScraper.SyncExchangeRatesAsync();
        if (result.IsFailure)
        {
            throw new Exception(result.Error.ToString());
        }
    }
}
