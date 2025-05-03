using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Investments.Queries.GetInvestments;
public class GetInvestmentsQuery : IRequest<Result<List<InvestmentResponse>>>
{
    public int PortfolioID { get; set; }
    public GetInvestmentsQuery(int PortfolioID)
    {
        this.PortfolioID = PortfolioID;
    }
    public class GetInvestmentsQueryHandler : IRequestHandler<GetInvestmentsQuery, Result<List<InvestmentResponse>>>
    {
        private readonly IInvestmentRepository investmentRepository;
        public GetInvestmentsQueryHandler(IInvestmentRepository investmentRepository)
        {
            this.investmentRepository = investmentRepository;
        }
        public async Task<Result<List<InvestmentResponse>>> Handle(GetInvestmentsQuery request, CancellationToken cancellationToken)
        {
            var investments = await investmentRepository.GetInvestmentsAsync(request.PortfolioID);

            return investments != null
                ? Result.Success(investments)
                : Result.Failure<List<InvestmentResponse>>(InvestmentErrors.InvestmentNotFound);
        }
    }
}
