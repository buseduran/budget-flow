using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Portfolios.Commands.CreatePortfolio;
public class CreatePortfolioCommand : IRequest<Result<int>>
{
    public PortfolioDto Portfolio { get; set; }
    public class CreatePortfolioCommandHandler : IRequestHandler<CreatePortfolioCommand, Result<int>>
    {
        private readonly IPortfolioRepository portfolioRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserWalletRepository userWalletRepository;
        public CreatePortfolioCommandHandler(IPortfolioRepository portfolioRepository, IHttpContextAccessor httpContextAccessor, IUserWalletRepository userWalletRepository)
        {
            this.portfolioRepository = portfolioRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.userWalletRepository = userWalletRepository;
        }
        public async Task<Result<int>> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var userWallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(request.Portfolio.WalletID, userID);
            if (userWallet == null)
                return Result.Failure<int>(UserWalletErrors.UserWalletNotFound);

            //check if the portfolio name exists
            var existingPortfolio = await portfolioRepository.GetPortfolioAsync(request.Portfolio.Name);
            if (existingPortfolio != null)
                return Result.Failure<int>(PortfolioErrors.PortfolioAlreadyExists);

            Portfolio portfolio = new()
            {
                Name = request.Portfolio.Name,
                Description = request.Portfolio.Description,
                WalletID = request.Portfolio.WalletID,
            };
            portfolio.UserID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            var result = await portfolioRepository.CreatePortfolioAsync(portfolio);
            return result == 0
                ? Result.Failure<int>(PortfolioErrors.PortfolioCreationFailed)
                : Result.Success(result);
        }
    }
}
