using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
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
        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            if (request.ID <= 0)
                return Result.Failure<bool>("Invalid category ID");

            var result = await categoryRepository.DeleteCategoryAsync(request.ID);
            return result
                ? Result.Success(true)
                : Result.Failure<bool>("Failed to delete category");
        }
    }
}
