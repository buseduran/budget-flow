using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Budget.Commands.DeleteEntry
{
    public class DeleteEntryCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public DeleteEntryCommand(int ID)
        {
            this.ID = ID;
        }
        public class DeleteEntryCommandHandler : IRequestHandler<DeleteEntryCommand, bool>
        {
            private readonly IBudgetRepository budgetRepository;
            public DeleteEntryCommandHandler(IBudgetRepository budgetRepository)
            {
                this.budgetRepository = budgetRepository;
            }
            public async Task<bool> Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
            {
                var result = await budgetRepository.DeleteEntryAsync(request.ID);
                if (!result)
                {
                    throw new Exception("Failed to delete entry.");
                }
                return true;
            }
        }

    }
}
