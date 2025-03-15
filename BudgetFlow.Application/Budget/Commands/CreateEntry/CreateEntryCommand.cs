using AutoMapper;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Commands.CreateEntry
{
    public class CreateEntryCommand : IRequest<bool>
    {
        public EntryDto Entry { get; set; }
        public class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, bool>
        {
            private readonly IBudgetRepository budgetRepository;
            private readonly IWalletRepository walletRepository;
            private readonly IMapper mapper;
            private readonly IHttpContextAccessor httpContextAccessor;
            public CreateEntryCommandHandler(IBudgetRepository budgetRepository, IWalletRepository walletRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                this.budgetRepository = budgetRepository;
                this.walletRepository = walletRepository;
                this.mapper = mapper;
                this.httpContextAccessor = httpContextAccessor;
            }

            public async Task<bool> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
            {
                var mappedEntry = mapper.Map<Entry>(request.Entry);
                GetCurrentUser getCurrentUser = new(httpContextAccessor);
                mappedEntry.UserID = getCurrentUser.GetCurrentUserID();

                var entryResult = await budgetRepository.CreateEntryAsync(mappedEntry);
                if (!entryResult)
                    throw new Exception("Failed to create entry.");

                if(mappedEntry.Type == EntryType.Income)
                    mappedEntry.Amount = Math.Abs(mappedEntry.Amount);
                else
                    mappedEntry.Amount = -Math.Abs(mappedEntry.Amount);

                var result= await walletRepository.UpdateWalletAsync(mappedEntry.UserID, mappedEntry.Amount);
                if (!result)
                    throw new Exception("Failed to update wallet.");

                return true;
            }
        }
    }
}
