using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
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
        private readonly ICategoryRepository categoryRepository;
        private readonly IBudgetRepository budgetRepository;
        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IBudgetRepository budgetRepository)
        {
            this.categoryRepository = categoryRepository;
            this.budgetRepository = budgetRepository;
        }
        public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            if (request.ID <= 0)
                return Result.Failure<bool>(CategoryErrors.InvalidCategoryId);

            #region Check if there is an existing entry 

            var budget = await budgetRepository.CheckEntryByCategoryAsync(request.ID);
            if (budget)
                return Result.Failure<bool>(CategoryErrors.CannotDeleteCategoryWithEntries);

            #endregion

            var result = await categoryRepository.DeleteCategoryAsync(request.ID);
            return result
                ? Result.Success(true)
                : Result.Failure<bool>(CategoryErrors.DeletionFailed);
        }
    }
}
