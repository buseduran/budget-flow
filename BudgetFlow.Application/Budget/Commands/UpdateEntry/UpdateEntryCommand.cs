using AutoMapper;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Budget.Commands.UpdateEntry;
public class UpdateEntryCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public EntryDto Entry { get; set; }
    public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand, Result<bool>>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateEntryCommandHandler(
            IBudgetRepository budgetRepository,
            ICurrentUserService currentUserService,
            IWalletRepository walletRepository,
            IMapper mapper,
            ICategoryRepository categoryRepository,
            IUserWalletRepository userWalletRepository,
            IUnitOfWork unitOfWork)
        {
            _budgetRepository = budgetRepository;
            _currentUserService = currentUserService;
            _walletRepository = walletRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _userWalletRepository = userWalletRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var mappedEntry = _mapper.Map<Entry>(request.Entry);
            mappedEntry.UserID = userID;

            var category = await _categoryRepository.GetCategoryByIdAsync(mappedEntry.CategoryID);

            #region Eski Tutar Üzerinden Farkı Hesapla ve Normalize Et
            var existingEntry = await _budgetRepository.GetEntryByIdAsync(request.ID);
            if (existingEntry == null)
                return Result.Failure<bool>(EntryErrors.EntryNotFound);

            var newAmount = category.Type == EntryType.Income
                ? Math.Abs(mappedEntry.Amount)
                : -Math.Abs(mappedEntry.Amount);

            var difference = newAmount - existingEntry.Amount;

            var wallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.Entry.WalletID, userID);
            if (category.Type == EntryType.Expense && wallet.Wallet.Balance < Math.Abs(difference) && difference < 0)
                return Result.Failure<bool>(WalletErrors.InsufficientBalance);
            #endregion

            #region Wallet ve Entry Güncelle
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _walletRepository.UpdateWalletAsync(userID, difference, saveChanges: false);

                mappedEntry.Amount = newAmount;

                await _budgetRepository.UpdateEntryAsync(request.ID, mappedEntry, saveChanges: false);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure<bool>(GeneralErrors.FromMessage(ex.Message));
            }
            #endregion
        }
    }
}
