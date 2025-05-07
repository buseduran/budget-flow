using BudgetFlow.Application.Categories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface ICategoryRepository
{
    Task<int> CreateCategoryAsync(Category categoryDto);
    Task<PaginatedList<CategoryResponse>> GetCategoriesAsync(int Page, int PageSize);
    Task<CategoryResponse> GetCategoryByIdAsync(int ID);
    Task<bool> UpdateCategoryAsync(int ID, string Color);
    Task<bool> DeleteCategoryAsync(int ID);
}
