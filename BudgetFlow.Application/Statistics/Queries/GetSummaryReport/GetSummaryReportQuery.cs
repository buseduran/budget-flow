using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public GetSummaryReportQueryHandler(ISummaryReportRepository summaryReportRepository)
        {
            _summaryReportRepository = summaryReportRepository;
        }
        public async Task<Result<SummaryReportResponse>> Handle(GetSummaryReportQuery request, CancellationToken cancellationToken)
        {
            var summaryReport = await _summaryReportRepository.GetByWalletIdAsync(request.WalletID);
            if (summaryReport == null)
                return Result.Failure<SummaryReportResponse>(WalletErrors.SummaryReportNotFound);

            return Result.Success(summaryReport);
        }
    }
}
