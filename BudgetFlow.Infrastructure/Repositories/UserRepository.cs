using AutoMapper;
using BudgetFlow.Application.Auth;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BudgetContext context;
    private readonly IMapper mapper;
    public UserRepository(BudgetContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<int> CreateAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user.ID;
    }

    public async Task<bool> DeleteAsync(int ID)
    {
        return await context.Users
            .Where(u => u.ID == ID)
            .ExecuteDeleteAsync() > 0;
    }

    public async Task<UserResponse> GetByIdAsync(int ID)
    {
        var user = await context.Users.FindAsync(ID);
        if (user == null) return null;
        var response = mapper.Map<UserResponse>(user);
        return response;
    }

    public async Task<UserResponse> GetByEmailAsync(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user == null) return null;
        var response = mapper.Map<UserResponse>(user);
        return response;
    }

    public async Task<bool> UpdateAsync(string Name, string OldEmail, string Email, string PasswordHash)
    {
        var user = await context.Users
            .Where(u => u.Email == OldEmail).FirstOrDefaultAsync();

        if (user == null) return false;
        user.Name = Name;
        user.Email = Email;
        user.PasswordHash = PasswordHash;

        var userDto = mapper.Map<User>(user);
        context.Users.Update(userDto);
        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> CreateRefreshToken(RefreshToken refreshToken)
    {
        context.RefreshTokens.Add(refreshToken);
        return await context.SaveChangesAsync() > 0;
    }
    public async Task<RefreshToken> GetRefreshToken(string token)
    {
        RefreshToken refreshToken = await context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);
        return refreshToken;
    }
    public async Task<RefreshToken> GetRefreshTokenByUserID(int userID)
    {
        RefreshToken refreshToken = await context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.UserID == userID);
        return refreshToken;
    }
    public async Task<bool> RevokeToken(int userID)
    {
        var count = await context.RefreshTokens
                            .Where(t => t.UserID == userID)
                            .CountAsync();
        if (count == 0)
        {
            return false;
        }
        await context.RefreshTokens
                   .Where(t => t.UserID == userID)
                   .ExecuteDeleteAsync();
        return true;
    }

    public async Task<bool> UpdateWithoutPassword(string Name, string OldEmail, string Email)
    {
        var user = await context.Users
            .Where(u => u.Email == OldEmail).FirstOrDefaultAsync();

        if (user == null) return false;
        user.Name = Name;
        user.Email = Email;

        var userDto = mapper.Map<User>(user);
        context.Users.Update(userDto);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<CurrencyType> GetUserCurrencyAsync(int UserID)
    {
        var currency = await context.Users
             .Where(u => u.ID == UserID)
             .Select(u => u.Wallet.Currency)
             .FirstOrDefaultAsync();
        return currency;
    }

    public async Task<CurrencyType> UpdateUserCurrencyAsync(int UserID, CurrencyType currencyType)
    {
        var user = await context.Users
            .Where(u => u.ID == UserID)
            .Include(u => u.Wallet)
            .FirstOrDefaultAsync();
        if (user == null) return CurrencyType.USD;
        user.Wallet.Currency = currencyType;
        await context.SaveChangesAsync();
        return currencyType;
    }
}