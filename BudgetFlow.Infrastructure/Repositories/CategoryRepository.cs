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

    public async Task<int> CreateCategoryAsync(Category category, bool saveChanges = true)
    {
        category.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        category.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        await context.Categories.AddAsync(category);
        if (saveChanges)
            await context.SaveChangesAsync();
        return category.ID;
    }

    public async Task<PaginatedList<CategoryResponse>> GetCategoriesAsync(int page, int pageSize, int walletID)
    {
        var categories = await context.Categories
            .Where(c => c.WalletID == walletID)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CategoryResponse
            {
                ID = c.ID,
                Name = c.Name,
                Color = c.Color,
                Type = c.Type
            })
            .ToListAsync();
        var count = await context.Categories.CountAsync();
        return new PaginatedList<CategoryResponse>(categories, count, page, pageSize);
    }

    public async Task<bool> UpdateCategoryAsync(int ID, string color, int walletID)
    {
        var category = await context.Categories
            .Where(c => c.ID == ID && c.WalletID == walletID)
            .FirstOrDefaultAsync();
        if (category is null) return false;

        category.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        category.Color = color;

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

    public async Task<CategoryResponse> GetCategoryByIDAsync(int ID)
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
