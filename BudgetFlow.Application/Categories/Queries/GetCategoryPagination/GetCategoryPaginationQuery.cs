using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Categories.Queries.GetCategoryPagination;
public class GetCategoryPaginationQuery : IRequest<Result<PaginatedList<CategoryResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public class GetCategoryPaginationQueryHandler : IRequestHandler<GetCategoryPaginationQuery, Result<PaginatedList<CategoryResponse>>>
    {
        private readonly ICategoryRepository categoryRepository;
        public GetCategoryPaginationQueryHandler(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public async Task<Result<PaginatedList<CategoryResponse>>> Handle(GetCategoryPaginationQuery request, CancellationToken cancellationToken)
        {
            var categories = await categoryRepository.GetCategoriesAsync(request.Page, request.PageSize);
            return categories != null
                ? Result.Success(categories)
                : Result.Failure<PaginatedList<CategoryResponse>>(CategoryErrors.CategoryNotFound);
        }
    }
}
