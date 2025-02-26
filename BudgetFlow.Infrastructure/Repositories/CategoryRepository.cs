using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;

namespace BudgetFlow.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BudgetContext context;
        private readonly IMapper mapper;
        public CategoryRepository(BudgetContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<bool> CreateCategoryAsync(CategoryDto Category)
        {
            Category.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            Category.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            
            await context.Categories.AddAsync(Category);
            return await context.SaveChangesAsync() > 0;
        }

    }
}
