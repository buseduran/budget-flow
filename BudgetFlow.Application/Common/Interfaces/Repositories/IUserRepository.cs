using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.User;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IUserRepository
{
    Task<bool> CreateAsync(UserModel entity);
    Task<UserResponse?> GetByIdAsync(int ID);
    Task<bool> UpdateAsync(int ID, UserModel entity);
    Task<bool> DeleteAsync(int ID);
}

