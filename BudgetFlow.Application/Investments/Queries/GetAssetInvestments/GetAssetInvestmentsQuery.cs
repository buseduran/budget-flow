using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Investments.Queries.GetAssetInvestments
{
    public class GetAssetInvestmentsQuery : IRequest<List<AssetInvestmentResponse>>
    {
        public string Portfolio { get; set; }
        public GetAssetInvestmentsQuery(string Portfolio)
        {
            this.Portfolio = Portfolio;
        }
        public class GetAssetInvestmentsQueryHandler : IRequestHandler<GetAssetInvestmentsQuery, List<AssetInvestmentResponse>>
        {
            private readonly IInvestmentRepository investmentRepository;
            private readonly IMapper mapper;
            public GetAssetInvestmentsQueryHandler(IInvestmentRepository investmentRepository, IMapper mapper)
            {
                this.investmentRepository = investmentRepository;
                this.mapper = mapper;
            }
            public async Task<List<AssetInvestmentResponse>> Handle(GetAssetInvestmentsQuery request, CancellationToken cancellationToken)
            {
                var investments = await investmentRepository.GetAssetInvestmentsAsync(request.Portfolio);
                return investments;
            }
        }
    }
}
