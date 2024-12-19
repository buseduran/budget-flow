using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.User;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IUserRepository
{
    Task<bool> CreateAsync(UserDto user);
    Task<UserResponse?> GetByIdAsync(int ID);
    Task<UserResponse> GetByEmailAsync(string email);
    Task<bool> UpdateAsync(int ID, UserRegisterModel entity);
    Task<bool> DeleteAsync(int ID);
}
