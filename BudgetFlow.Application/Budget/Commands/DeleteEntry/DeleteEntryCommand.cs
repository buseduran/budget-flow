using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.Budget.Commands.DeleteEntry;
public class DeleteEntryCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public DeleteEntryCommand(int ID)
    {
        this.ID = ID;
    }
    public class DeleteEntryCommandHandler : IRequestHandler<DeleteEntryCommand, Result<bool>>
    {
        private readonly IBudgetRepository budgetRepository;
        public DeleteEntryCommandHandler(IBudgetRepository budgetRepository)
        {
            this.budgetRepository = budgetRepository;
        }
        public async Task<Result<bool>> Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
        {
            var result = await budgetRepository.DeleteEntryAsync(request.ID);
            if (!result)
            {
                return Result.Failure<bool>("Failed to delete the entry.");
            }
            return Result.Success(true);
        }
    }

}

