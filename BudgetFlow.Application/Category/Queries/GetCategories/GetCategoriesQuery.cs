using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Category.Queries.GetCategories
{
    public class GetCategoriesQuery : IRequest<IEnumerable<CategoryResponse>>
    {
        public class GetCategoriesQueryHandler:IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryResponse>>
        {
            private readonly ICategoryRepository categoryRepository;
            public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
            {
                this.categoryRepository = categoryRepository;
            }
            public async Task<IEnumerable<CategoryResponse>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
            {
                var categories = await categoryRepository.GetCategoriesAsync();
                return categories;
            }
        }
    }
}
