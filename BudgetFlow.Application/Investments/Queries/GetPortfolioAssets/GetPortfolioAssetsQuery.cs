using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Investments.Queries
{
    public class GetPortfolioAssetsQuery : IRequest<List<PortfolioAssetResponse>>
    {
        public string Portfolio { get; set; }
        public GetPortfolioAssetsQuery(string Portfolio)
        {
            this.Portfolio = Portfolio;
        }
        public class GetPortfolioAssetsQueryHandler : IRequestHandler<GetPortfolioAssetsQuery, List<PortfolioAssetResponse>>
        { 
            private readonly IInvestmentRepository investmentRepository;
            private readonly IMapper mapper;
            public GetPortfolioAssetsQueryHandler(IInvestmentRepository investmentRepository, IMapper mapper)
            {
                this.investmentRepository = investmentRepository;
                this.mapper = mapper;
            }
            public async Task<List<PortfolioAssetResponse>> Handle(GetPortfolioAssetsQuery request, CancellationToken cancellationToken)
            {
                var investments = await investmentRepository.GetAssetInvestmentsAsync(request.Portfolio);
                return investments;
            }
        }
    }
}
