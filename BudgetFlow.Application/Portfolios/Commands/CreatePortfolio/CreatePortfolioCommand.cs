using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Portfolios.Commands.CreatePortfolio;
public class CreatePortfolioCommand : IRequest<Result<int>>
{
    public PortfolioDto Portfolio { get; set; }
    public class CreatePortfolioCommandHandler : IRequestHandler<CreatePortfolioCommand, Result<int>>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserWalletRepository _userWalletRepository;
        public CreatePortfolioCommandHandler(
            IPortfolioRepository portfolioRepository,
            ICurrentUserService currentUserService,
            IUserWalletRepository userWalletRepository)
        {
            _portfolioRepository = portfolioRepository;
            _currentUserService = currentUserService;
            _userWalletRepository = userWalletRepository;
        }
        public async Task<Result<int>> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var userWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.Portfolio.WalletID, userID);
            if (userWallet == null)
                return Result.Failure<int>(UserWalletErrors.UserWalletNotFound);

            //check if the portfolio name exists
            var existingPortfolio = await _portfolioRepository.GetPortfolioAsync(request.Portfolio.Name, userID);
            if (existingPortfolio != null)
                return Result.Failure<int>(PortfolioErrors.PortfolioAlreadyExists);

            Portfolio portfolio = new()
            {
                Name = request.Portfolio.Name,
                Description = request.Portfolio.Description,
                WalletID = request.Portfolio.WalletID,
            };
            portfolio.UserID = userID;

            var result = await _portfolioRepository.CreatePortfolioAsync(portfolio);
            return result == 0
                ? Result.Failure<int>(PortfolioErrors.PortfolioCreationFailed)
                : Result.Success(result);
        }
    }
}
