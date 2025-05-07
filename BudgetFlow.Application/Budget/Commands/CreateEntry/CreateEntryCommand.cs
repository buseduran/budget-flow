using AutoMapper;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Commands.CreateEntry;
public class CreateEntryCommand : IRequest<Result<bool>>
{
    public EntryDto Entry { get; set; }
    public class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, Result<bool>>
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IWalletRepository walletRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        public CreateEntryCommandHandler(IBudgetRepository budgetRepository, IWalletRepository walletRepository, ICategoryRepository categoryRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.budgetRepository = budgetRepository;
            this.walletRepository = walletRepository;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<bool>> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            #region Check user has a wallet
            var wallet = await walletRepository.GetWalletAsync(userID);
            if (wallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);
            #endregion

            var mappedEntry = mapper.Map<Entry>(request.Entry);
            mappedEntry.UserID = userID;

            var category = await categoryRepository.GetCategoryByIdAsync(mappedEntry.CategoryID);
            if (category is null)
                return Result.Failure<bool>(CategoryErrors.CategoryNotFound);

            #region Check user's wallet balance is enough
            if ((category.Type == EntryType.Expense) && (wallet.Balance < Math.Abs(mappedEntry.Amount)))
                return Result.Failure<bool>(WalletErrors.InsufficientBalance);
            #endregion

            #region Create entry and Update wallet
            var entryResult = await budgetRepository.CreateEntryAsync(mappedEntry);
            if (!entryResult)
                return Result.Failure<bool>(EntryErrors.CreationFailed);

            if (category.Type == EntryType.Income)
                mappedEntry.Amount = Math.Abs(mappedEntry.Amount);
            else
                mappedEntry.Amount = -Math.Abs(mappedEntry.Amount);

            var result = await walletRepository.UpdateWalletAsync(mappedEntry.UserID, mappedEntry.Amount);
            if (!result)
                return Result.Failure<bool>(WalletErrors.UpdateFailed);
            #endregion

            return Result.Success(true);
        }
    }
}
