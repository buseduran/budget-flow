using BudgetFlow.Application.Categories;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly BudgetContext context;
    public CategoryRepository(BudgetContext context)
    {
        this.context = context;
    }

    public async Task<int> CreateCategoryAsync(Category Category, bool saveChanges = true)
    {
        Category.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        Category.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        await context.Categories.AddAsync(Category);
        if (saveChanges)
            await context.SaveChangesAsync();
        return Category.ID;
    }

    public async Task<PaginatedList<CategoryResponse>> GetCategoriesAsync(int Page, int PageSize, int userID)
    {
        var categories = await context.Categories
            .Where(c => c.UserID == userID)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .Select(c => new CategoryResponse
            {
                ID = c.ID,
                Name = c.Name,
                Color = c.Color,
                Type = c.Type
            })
            .ToListAsync();
        var count = await context.Categories.CountAsync();
        return new PaginatedList<CategoryResponse>(categories, count, Page, PageSize);
    }

    public async Task<bool> UpdateCategoryAsync(int ID, string Color, int UserID)
    {
        var category = await context.Categories
            .Where(c => c.ID == ID && c.UserID == UserID)
            .FirstOrDefaultAsync();
        if (category is null) return false;

        category.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        category.Color = Color;

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCategoryAsync(int ID)
    {
        var category = await context.Categories.FindAsync(ID);
        if (category is null) return false;

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<CategoryResponse> GetCategoryByIdAsync(int ID)
    {
        return await context.Categories
            .Where(c => c.ID == ID)
            .Select(c => new CategoryResponse
            {
                ID = c.ID,
                Name = c.Name,
                Color = c.Color,
                Type = c.Type
            })
            .FirstOrDefaultAsync();
    }
}
