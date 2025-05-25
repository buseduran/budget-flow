using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Categories.Queries.GetCategoryPagination;
public class GetCategoryPaginationQuery : IRequest<Result<PaginatedList<CategoryResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public class GetCategoryPaginationQueryHandler : IRequestHandler<GetCategoryPaginationQuery, Result<PaginatedList<CategoryResponse>>>
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetCategoryPaginationQueryHandler(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.categoryRepository = categoryRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<PaginatedList<CategoryResponse>>> Handle(GetCategoryPaginationQuery request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var categories = await categoryRepository.GetCategoriesAsync(request.Page, request.PageSize, userID);
            return categories != null
                ? Result.Success(categories)
                : Result.Failure<PaginatedList<CategoryResponse>>(CategoryErrors.CategoryNotFound);
        }
    }
}
