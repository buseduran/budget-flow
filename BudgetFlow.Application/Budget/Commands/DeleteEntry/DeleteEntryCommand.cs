using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
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
        private readonly IBudgetRepository _budgetRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEntryCommandHandler(
            IBudgetRepository budgetRepository,
            IWalletRepository walletRepository,
            IUserWalletRepository userWalletRepository,
            ICategoryRepository categoryRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _budgetRepository = budgetRepository;
            _walletRepository = walletRepository;
            _userWalletRepository = userWalletRepository;
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();

            var existingEntry = await _budgetRepository.GetEntryByIdAsync(request.ID);
            if (existingEntry == null)
                return Result.Failure<bool>(EntryErrors.EntryNotFound);

            var category = await _categoryRepository.GetCategoryByIdAsync(existingEntry.Category.ID);
            if (category == null)
                return Result.Failure<bool>(CategoryErrors.CategoryNotFound);

            var wallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(existingEntry.WalletID, userID);
            if (wallet == null)
                return Result.Failure<bool>(EntryErrors.EntryNotFound); // veya WalletErrors.WalletNotFound

            decimal balanceAdjustment = category.Type == EntryType.Income
                ? -Math.Abs(existingEntry.Amount)
                : Math.Abs(existingEntry.Amount);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var walletUpdateResult = await _walletRepository.UpdateWalletAsync(wallet.WalletID, balanceAdjustment, saveChanges: false);

                var deleteResult = await _budgetRepository.DeleteEntryAsync(request.ID, saveChanges: false);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure<bool>(GeneralErrors.FromMessage(ex.Message));
            }
        }
    }
}
