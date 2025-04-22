using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.Investments.Commands.DeleteInvestment;
public class DeleteInvestmentCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public DeleteInvestmentCommand(int ID)
    {
        this.ID = ID;
    }
    public class DeleteInvestmentCommandHandler : IRequestHandler<DeleteInvestmentCommand, Result<bool>>
    {
        private readonly IInvestmentRepository investmentRepository;
        public DeleteInvestmentCommandHandler(IInvestmentRepository investmentRepository)
        {
            this.investmentRepository = investmentRepository;
        }
        public async Task<Result<bool>> Handle(DeleteInvestmentCommand request, CancellationToken cancellationToken)
        {
            if (request.ID <= 0)
            {
                return Result.Failure<bool>("Invalid Investment ID");
            }

            var result = await investmentRepository.DeleteInvestmentAsync(request.ID);
            return result
                ? Result.Success(result)
                : Result.Failure<bool>("Failed to delete investment");
        }
    }
}
