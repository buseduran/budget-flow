using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Models;
using MediatR;

namespace BudgetFlow.Application.Budget.Commands.UpdateEntry
{
    public class UpdateEntryCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public EntryModel Entry { get; set; }
        public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand, bool>
        {
            private readonly IBudgetRepository budgetRepository;
            public UpdateEntryCommandHandler(IBudgetRepository budgetRepository)
            {
                this.budgetRepository = budgetRepository;
            }
            public async Task<bool> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
            {
                var result = await budgetRepository.UpdateEntryAsync(request.ID, request.Entry);
                if (!result)
                {
                    throw new Exception("Failed to update entry.");
                }
                return true;
            }
        }
    }
}
