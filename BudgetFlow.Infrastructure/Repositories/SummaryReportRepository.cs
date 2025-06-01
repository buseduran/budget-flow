using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;

public class SummaryReportRepository : ISummaryReportRepository
{
    private readonly BudgetContext _context;

    public SummaryReportRepository(BudgetContext context)
    {
        _context = context;
    }

    public async Task<SummaryReportResponse> GetByWalletIdAsync(int walletId)
    {
        return await _context.SummaryReports
            .Where(report => report.WalletID == walletId)
            .Select(report => new SummaryReportResponse
            {
                Analysis = report.Analysis,
                AnalysisDate = report.AnalysisDate
            }).FirstOrDefaultAsync();
    }

    public async Task<bool> CreateOrUpdateAsync(SummaryReport report)
    {
        var existingReport = await GetByWalletIdAsync(report.WalletID);
        SummaryReport summaryReport = new SummaryReport
        {
            WalletID = report.WalletID,
            Analysis = report.Analysis,
            AnalysisDate = DateTime.UtcNow
        };

        if (existingReport == null)
        {
            report.CreatedAt = DateTime.UtcNow;
            report.UpdatedAt = DateTime.UtcNow;
            await _context.SummaryReports.AddAsync(report);
        }
        else
        {
            //existingReport.Analysis = report.Analysis;
            //existingReport.AnalysisDate = DateTime.UtcNow;
            summaryReport.Analysis = report.Analysis;
            summaryReport.AnalysisDate = DateTime.UtcNow;



            _context.SummaryReports.Update(summaryReport);
        }
        await _context.SaveChangesAsync();
        return true;
    }
}