using BudgetFlow.Application.Auth;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IUserRepository
{
    Task<bool> CreateAsync(UserDto user);
    Task<UserResponse?> GetByIdAsync(int ID);
    Task<UserResponse> GetByEmailAsync(string email);
    Task<bool> UpdateAsync(string Name, string Email, string PasswordHash);
    Task<bool> DeleteAsync(int ID);
    Task<bool> CreateRefreshToken(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshToken(string token);
    Task<RefreshToken> GetRefreshTokenByUserID(int userID);
    Task<bool> RevokeToken(int userID);
}
