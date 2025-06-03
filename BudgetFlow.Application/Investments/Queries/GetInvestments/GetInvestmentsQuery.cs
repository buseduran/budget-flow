using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Investments.Queries.GetInvestments;
public class GetInvestmentsQuery : IRequest<Result<PaginatedList<InvestmentPaginationResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int PortfolioID { get; set; }
    public int? AssetId { get; set; }
    public class GetInvestmentsQueryHandler : IRequestHandler<GetInvestmentsQuery, Result<PaginatedList<InvestmentPaginationResponse>>>
    {
        private readonly IInvestmentRepository investmentRepository;
        public GetInvestmentsQueryHandler(IInvestmentRepository investmentRepository)
        {
            this.investmentRepository = investmentRepository;
        }
        public async Task<Result<PaginatedList<InvestmentPaginationResponse>>> Handle(GetInvestmentsQuery request, CancellationToken cancellationToken)
        {
            var investments = await investmentRepository.GetInvestmentsAsync(request.Page, request.PageSize, request.PortfolioID, request.AssetId);

            return investments != null
                ? Result.Success(investments)
                : Result.Failure<PaginatedList<InvestmentPaginationResponse>>(InvestmentErrors.InvestmentNotFound);
        }
    }
}
