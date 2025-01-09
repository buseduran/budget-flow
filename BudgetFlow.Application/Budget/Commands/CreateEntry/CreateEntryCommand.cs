using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Commands.CreateEntry
{
    public class CreateEntryCommand : IRequest<bool>
    {
        public EntryModel Entry { get; set; }
        public class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, bool>
        {
            private readonly IBudgetRepository budgetRepository;
            private readonly IMapper mapper;
            private readonly IHttpContextAccessor httpContextAccessor;
            public CreateEntryCommandHandler(IBudgetRepository budgetRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                this.budgetRepository = budgetRepository;
                this.mapper = mapper;
                this.httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
            {
                var mappedEntry = mapper.Map<EntryDto>(request.Entry);
                GetCurrentUser getCurrentUser = new(httpContextAccessor);
                mappedEntry.UserID = getCurrentUser.GetCurrentUserID();

                var result = await budgetRepository.CreateEntryAsync(mappedEntry);
                if (!result)
                    throw new Exception("Failed to create entry.");
                return true;
            }
        }
    }
}
