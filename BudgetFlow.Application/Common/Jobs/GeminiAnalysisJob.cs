using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Quartz;

namespace BudgetFlow.Application.Common.Jobs;

public class GeminiAnalysisJob : IJob
{
    private readonly IGeminiService _geminiService;
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly ILogger<GeminiAnalysisJob> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IWalletRepository _walletRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISummaryReportRepository _summaryReportRepository;

    public GeminiAnalysisJob(
        IGeminiService geminiService,
        IStatisticsRepository statisticsRepository,
        ILogger<GeminiAnalysisJob> logger,
        ICurrentUserService currentUserService,
        IWalletRepository walletRepository,
        IUserRepository userRepository,
        ISummaryReportRepository summaryReportRepository)
    {
        _geminiService = geminiService;
        _statisticsRepository = statisticsRepository;
        _logger = logger;
        _currentUserService = currentUserService;
        _walletRepository = walletRepository;
        _userRepository = userRepository;
        _summaryReportRepository = summaryReportRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            // Get all user and their wallets
            var users = await _userRepository.GetPaginatedAsync(1, 1000);

            foreach (var user in users.Items)
            {
                var userID = user.ID;
                var wallets = await _walletRepository.GetWalletsAsync(userID);

                foreach (var wallet in wallets)
                {
                    try
                    {
                        // Get budget data from repository
                        var budgetData = await _statisticsRepository.GetAnalysisEntriesAsync("1m", wallet.ID);

                        // Create analysis request
                        var request = new BudgetAnalysisRequest
                        {
                            BudgetData = budgetData,
                            AnalysisType = "daily",
                            StartDate = DateTime.UtcNow.AddDays(-1),
                            EndDate = DateTime.UtcNow
                        };

                        // Generate analysis using Gemini
                        var analysis = await _geminiService.GenerateBudgetAnalysisAsync(request);

                        // Save analysis to database
                        var summaryReport = new SummaryReport
                        {
                            WalletID = wallet.ID,
                            Analysis = analysis,
                            AnalysisDate = DateTime.UtcNow
                        };

                        await _summaryReportRepository.CreateOrUpdateAsync(summaryReport);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing wallet {WalletId} for user {UserId}", wallet.ID, userID);
                        continue;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GeminiAnalysisJob");
            throw;
        }
    }
}