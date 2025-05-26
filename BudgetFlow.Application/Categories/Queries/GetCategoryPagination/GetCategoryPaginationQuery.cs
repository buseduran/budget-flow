using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
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
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetCategoryPaginationQueryHandler(ICategoryRepository categoryRepository, ICurrentUserService currentUserService)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
        }
        public async Task<Result<PaginatedList<CategoryResponse>>> Handle(GetCategoryPaginationQuery request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var categories = await _categoryRepository.GetCategoriesAsync(request.Page, request.PageSize, userID);
            return categories != null
                ? Result.Success(categories)
                : Result.Failure<PaginatedList<CategoryResponse>>(CategoryErrors.CategoryNotFound);
        }
    }
}
