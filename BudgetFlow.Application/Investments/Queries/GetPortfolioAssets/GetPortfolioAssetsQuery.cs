using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Investments.Queries;
public class GetPortfolioAssetsQuery : IRequest<Result<PortfolioAssetResponse>>
{
    public string Portfolio { get; set; }
    public GetPortfolioAssetsQuery(string Portfolio)
    {
        this.Portfolio = Portfolio;
    }
    public class GetPortfolioAssetsQueryHandler : IRequestHandler<GetPortfolioAssetsQuery, Result<PortfolioAssetResponse>>
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetPortfolioAssetsQueryHandler(IInvestmentRepository investmentRepository, ICurrentUserService currentUserService)
        {
            _investmentRepository = investmentRepository;
            _currentUserService = currentUserService;
        }
        public async Task<Result<PortfolioAssetResponse>> Handle(GetPortfolioAssetsQuery request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var investments = await _investmentRepository.GetAssetInvestmentsAsync(request.Portfolio, userID);

            return investments != null
                ? Result.Success(investments)
                : Result.Failure<PortfolioAssetResponse>(InvestmentErrors.InvestmentNotFound);
        }
    }
}
