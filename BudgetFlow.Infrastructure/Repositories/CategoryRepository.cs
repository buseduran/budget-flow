using AutoMapper;
using BudgetFlow.Application.Category;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BudgetContext context;
        public CategoryRepository(BudgetContext context)
        {
            this.context = context;
        }

        public async Task<bool> CreateCategoryAsync(CategoryEntity Category)
        {
            Category.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            Category.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            await context.Categories.AddAsync(Category);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<CategoryResponse>> GetCategoriesAsync()
        {
            return await context.Categories
                .OrderByDescending(c => c.CreatedAt) 
                .Select(c => new CategoryResponse
                {
                    ID = c.ID,
                    Name = c.Name,
                    Color = c.Color
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateCategoryAsync(int ID, string Color)
        {
            var category = await context.Categories.FindAsync(ID);
            if (category is null) return false;

            category.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            category.Color = Color;

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int ID)
        {
            return await context.Categories
                    .Where(e => e.ID == ID)
                    .ExecuteDeleteAsync() > 0;
        }
    }
}
