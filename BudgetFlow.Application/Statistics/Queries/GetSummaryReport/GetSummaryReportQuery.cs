using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Statistics.Queries.GetSummaryReport;
public class GetSummaryReportQuery : IRequest<Result<SummaryReportResponse>>
{
    public int WalletID { get; set; }
    public GetSummaryReportQuery(int walletID)
    {
        WalletID = walletID;
    }

    public class GetSummaryReportQueryHandler : IRequestHandler<GetSummaryReportQuery, Result<SummaryReportResponse>>
    {
        private readonly ISummaryReportRepository _summaryReportRepository;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IGeminiService _geminiService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserWalletRepository _userWalletRepository;
        public GetSummaryReportQueryHandler(
            ISummaryReportRepository summaryReportRepository,
            IStatisticsRepository statisticsRepository,
            IGeminiService geminiService,
            ICurrentUserService currentUserService,
            IUserWalletRepository userWalletRepository)
        {
            _summaryReportRepository = summaryReportRepository;
            _statisticsRepository = statisticsRepository;
            _geminiService = geminiService;
            _currentUserService = currentUserService;
            _userWalletRepository = userWalletRepository;
        }
        public async Task<Result<SummaryReportResponse>> Handle(GetSummaryReportQuery request, CancellationToken cancellationToken)
        {
            var existingSummaryReport = await _summaryReportRepository.GetByWalletIdAsync(request.WalletID);
            if (existingSummaryReport == null)
            {
                var userID = _currentUserService.GetCurrentUserID();
                var userWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
                if (userWallet == null)
                    return Result.Failure<SummaryReportResponse>(UserWalletErrors.UserWalletNotFound);

                // Get budget data from repository
                // TODO: Uncomment and implement the budget data retrieval logic
                var budgetData = await _statisticsRepository.GetAnalysisEntriesAsync(userID, "1m", userWallet.WalletID);
                //if (budgetData == null || !budgetData.Any())
                //    return Result.Failure<SummaryReportResponse>(StatisticsErrors.NoBudgetDataFound);

                // Create analysis request
                var analysisRequest = new BudgetAnalysisRequest
                {
                    BudgetData = budgetData,
                    AnalysisType = "daily",
                    StartDate = DateTime.SpecifyKind(DateTime.Now.AddDays(-1), DateTimeKind.Utc),
                    EndDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                };

                // Generate analysis using Gemini
                var analysis = await _geminiService.GenerateBudgetAnalysisAsync(analysisRequest);

                // Save analysis to database
                var summaryReport = new SummaryReport
                {
                    WalletID = userWallet.WalletID,
                    Analysis = analysis,
                    AnalysisDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                };

                await _summaryReportRepository.CreateOrUpdateAsync(summaryReport);

                return Result.Success(new SummaryReportResponse
                {
                    Analysis = summaryReport.Analysis,
                    AnalysisDate = summaryReport.AnalysisDate
                });
            }
            return Result.Success(new SummaryReportResponse
            {
                Analysis = existingSummaryReport.Analysis,
                AnalysisDate = existingSummaryReport.AnalysisDate
            });
        }
    }
}
