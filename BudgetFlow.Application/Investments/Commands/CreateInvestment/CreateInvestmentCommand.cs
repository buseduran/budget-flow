using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using MediatR;

namespace BudgetFlow.Application.Investments.Commands.CreateInvestment
{
    public class CreateInvestmentCommand : IRequest<bool>
    {
        public InvestmentDto Investment { get; set; }
        public class CreateInvestmentCommandHandler : IRequestHandler<CreateInvestmentCommand, bool>
        {
            private readonly IInvestmentRepository investmentRepository;
            public CreateInvestmentCommandHandler(IInvestmentRepository investmentRepository)
            {
                this.investmentRepository = investmentRepository;
            }

            public async Task<bool> Handle(CreateInvestmentCommand request, CancellationToken cancellationToken)
            {
                var investment = new Investment
                {
                    AssetId = request.Investment.AssetId,
                    Amount = request.Investment.Amount,
                    PurchaseDate = request.Investment.PurchaseDate,
                    PortfolioId = request.Investment.PortfolioId
                };
                var result = await investmentRepository.CreateInvestmentAsync(investment);
                return result;
            }
        }
    }
}
