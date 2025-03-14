using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Investments.Queries.GetLastInvestments
{
    public class GetLastInvestmentsQuery : IRequest<List<LastInvestmentResponse>>
    {
        public string Portfolio { get; set; }
        public GetLastInvestmentsQuery(string Portfolio)
        {
            this.Portfolio = Portfolio;
        }
        public class GetLastInvestmentsQueryHandler : IRequestHandler<GetLastInvestmentsQuery, List<LastInvestmentResponse>>
        {
            private readonly IInvestmentRepository investmentRepository;
            private readonly IMapper mapper;
            public GetLastInvestmentsQueryHandler(IInvestmentRepository investmentRepository, IMapper mapper)
            {
                this.investmentRepository = investmentRepository;
                this.mapper = mapper;
            }
            public async Task<List<LastInvestmentResponse>> Handle(GetLastInvestmentsQuery request, CancellationToken cancellationToken)
            {
                var investments = await investmentRepository.GetLastInvestmentsAsync(request.Portfolio);
                return investments;
            }
        }
    }
}
