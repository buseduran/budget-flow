using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Categories.Commands.DeleteCategory;
public class DeleteCategoryCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public DeleteCategoryCommand(int ID)
    {
        this.ID = ID;
    }
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly ICurrentUserService _currentUserService;
        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IBudgetRepository budgetRepository, ICurrentUserService currentUserService)
        {
            _categoryRepository = categoryRepository;
            _budgetRepository = budgetRepository;
            _currentUserService = currentUserService;
        }
        public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            if (request.ID <= 0)
                return Result.Failure<bool>(CategoryErrors.InvalidCategoryId);

            #region Check if there is an existing entry 
            var userID = _currentUserService.GetCurrentUserID();
            var budget = await _budgetRepository.CheckEntryByCategoryAsync(request.ID, userID);
            if (budget)
                return Result.Failure<bool>(CategoryErrors.CannotDeleteCategoryWithEntries);
            #endregion

            var result = await _categoryRepository.DeleteCategoryAsync(request.ID);
            return result
                ? Result.Success(true)
                : Result.Failure<bool>(CategoryErrors.DeletionFailed);
        }
    }
}
