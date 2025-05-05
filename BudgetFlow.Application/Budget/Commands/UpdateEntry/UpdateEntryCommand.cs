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

namespace BudgetFlow.Application.Budget.Commands.UpdateEntry;
public class UpdateEntryCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public EntryDto Entry { get; set; }
    public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand, Result<bool>>
    {
        private readonly IBudgetRepository budgetRepository;
        private readonly IWalletRepository walletRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;
        public UpdateEntryCommandHandler(
            IBudgetRepository budgetRepository,
            IHttpContextAccessor httpContextAccessor,
            IWalletRepository walletRepository,
            IMapper mapper,
            ICategoryRepository categoryRepository)
        {
            this.budgetRepository = budgetRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.walletRepository = walletRepository;
            this.mapper = mapper;
            this.categoryRepository = categoryRepository;
        }
        public async Task<Result<bool>> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var mappedEntry = mapper.Map<Entry>(request.Entry);
            mappedEntry.UserID = userID;

            var category = await categoryRepository.GetCategoryByIdAsync(mappedEntry.CategoryID);

            #region Eski Tutar Üzerinden Farkı Hesapla ve Normalize Et
            var existingEntry = await budgetRepository.GetEntryByIdAsync(request.ID);
            if (existingEntry == null)
                return Result.Failure<bool>(EntryErrors.EntryNotFound);

            var newAmount = category.Type == EntryType.Income
                ? Math.Abs(mappedEntry.Amount)
                : -Math.Abs(mappedEntry.Amount);

            var difference = newAmount - existingEntry.Amount;
            var wallet = await walletRepository.GetWalletAsync(userID);
            if (category.Type == EntryType.Expense && wallet.Balance < Math.Abs(difference) && difference < 0)
                return Result.Failure<bool>(WalletErrors.InsufficientBalance);

            var walletUpdateResult = await walletRepository.UpdateWalletAsync(userID, difference);
            if (!walletUpdateResult)
                return Result.Failure<bool>(WalletErrors.UpdateFailed);
            #endregion

            #region Entry Güncelle
            mappedEntry.Amount = newAmount;
            var entryResult = await budgetRepository.UpdateEntryAsync(request.ID, mapper.Map<EntryDto>(mappedEntry));
            if (!entryResult)
                return Result.Failure<bool>(EntryErrors.EntryUpdateFailed);
            #endregion

            return Result.Success(true);
        }
    }
}
