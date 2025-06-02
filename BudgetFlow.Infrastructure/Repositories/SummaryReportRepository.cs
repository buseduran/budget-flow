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
            AnalysisDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
        };

        if (existingReport == null)
        {
            report.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            report.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            await _context.SummaryReports.AddAsync(report);
        }
        else
        {
            summaryReport.Analysis = report.Analysis;
            summaryReport.AnalysisDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            _context.SummaryReports.Update(summaryReport);
        }
        await _context.SaveChangesAsync();

        return true;
    }
}