using BudgetFlow.Application.Common.Models;
using MediatR;

namespace BudgetFlow.Application.Budget.Commands.CreateEntry
{
    public class CreateEntryCommand : IRequest<bool>
    {
        public EntryModel Entry { get; set; }
        public class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, bool>
        {

            public async Task<bool> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
            {
                return true;
            }
        }


    }
}
