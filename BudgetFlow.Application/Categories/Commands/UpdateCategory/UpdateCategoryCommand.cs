using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.Categories.Commands.UpdateCategory;
public class UpdateCategoryCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public string Color { get; set; }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<bool>>
    {
        private readonly ICategoryRepository categoryRepository;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<Result<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var result = await categoryRepository.UpdateCategoryAsync(request.ID, request.Color);
            return result
                ? Result.Success(true)
                : Result.Failure<bool>("Failed to update category");
        }
    }
}
