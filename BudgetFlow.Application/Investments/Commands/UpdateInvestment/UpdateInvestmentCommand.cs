using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.Investments.Commands.UpdateInvestment;
public class UpdateInvestmentCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public InvestmentDto Investment { get; set; }
    public class UpdateInvestmentCommandHandler : IRequestHandler<UpdateInvestmentCommand, Result<bool>>
    {
        private readonly IInvestmentRepository repository;
        public UpdateInvestmentCommandHandler(IInvestmentRepository repository)
        {
            this.repository = repository;
        }
        public async Task<Result<bool>> Handle(UpdateInvestmentCommand request, CancellationToken cancellationToken)
        {
            if (request.ID <= 0)
                return Result.Failure<bool>("Investment ID must be greater than zero");

            var result = await repository.UpdateInvestmentAsync(request.ID, request.Investment);
            return result
                ? Result.Success(result)
                : Result.Failure<bool>("Failed to update investment");
        }
    }
}
