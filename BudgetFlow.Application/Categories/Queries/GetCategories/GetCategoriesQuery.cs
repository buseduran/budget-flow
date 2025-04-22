using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.Categories.Queries.GetCategories;
public class GetCategoriesQuery : IRequest<Result<List<CategoryResponse>>>
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryResponse>>>
    {
        private readonly ICategoryRepository categoryRepository;
        public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public async Task<Result<List<CategoryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await categoryRepository.GetCategoriesAsync();
            return categories != null
                ? Result.Success(categories.ToList())
                : Result.Failure<List<CategoryResponse>>("Failed to retrieve categories");
        }
    }
}
