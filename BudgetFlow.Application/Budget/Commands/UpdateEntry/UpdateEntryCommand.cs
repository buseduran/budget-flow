using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.Budget.Commands.UpdateEntry;
public class UpdateEntryCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public EntryDto Entry { get; set; }
    public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand, Result<bool>>
    {
        private readonly IBudgetRepository budgetRepository;
        public UpdateEntryCommandHandler(IBudgetRepository budgetRepository)
        {
            this.budgetRepository = budgetRepository;
        }
        public async Task<Result<bool>> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
        {
            var result = await budgetRepository.UpdateEntryAsync(request.ID, request.Entry);
            if (!result)
            {
                return Result.Failure<bool>("Failed to update entry");
            }
            return Result.Success(true);
        }
    }
}

