using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Investments.Queries.GetInvestments
{
    public class GetInvestmentsQuery : IRequest<List<InvestmentResponse>>
    {
        public int PortfolioID { get; set; }
        public GetInvestmentsQuery(int PortfolioID)
        {
            this.PortfolioID = PortfolioID;
        }

        public class GetInvestmentsQueryHandler : IRequestHandler<GetInvestmentsQuery, List<InvestmentResponse>>
        {
            private readonly IInvestmentRepository investmentRepository;
            public GetInvestmentsQueryHandler(IInvestmentRepository investmentRepository)
            {
                this.investmentRepository = investmentRepository;
            }
            public async Task<List<InvestmentResponse>> Handle(GetInvestmentsQuery request, CancellationToken cancellationToken)
            {
                var investments = await investmentRepository.GetInvestmentsAsync(request.PortfolioID);
                return investments;
            }
        }
    }
}
