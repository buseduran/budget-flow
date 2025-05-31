using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Users;
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

    public async Task<int> CreateAsync(User user, bool saveChanges = true)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await context.Users.AddAsync(user);
        if (saveChanges)
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
        var user = await context.Users
         .Include(u => u.UserWallets)
             .ThenInclude(uw => uw.Wallet)
         .FirstOrDefaultAsync(u => u.ID == ID);

        if (user is null) return null;

        var response = new UserResponse
        {
            ID = user.ID,
            Name = user.Name,
            Email = user.Email,
            IsEmailConfirmed = user.IsEmailConfirmed,
            PasswordHash = user.PasswordHash,
            Wallets = user.UserWallets.Select(uw => new UserWalletResponse
            {
                WalletID = uw.WalletID,
                Role = uw.Role,
                Balance = uw.Wallet.Balance
            }).ToList()
        };

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
    public async Task<bool> CreateRefreshTokenAsync(RefreshToken refreshToken)
    {
        context.RefreshTokens.Add(refreshToken);
        return await context.SaveChangesAsync() > 0;
    }
    public async Task<RefreshToken> GetRefreshTokenAsync(string token)
    {
        RefreshToken refreshToken = await context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);
        return refreshToken;
    }
    public async Task<bool> UpdateRefreshTokenAsync(RefreshToken refreshToken)
    {
        context.RefreshTokens.Update(refreshToken);
        return await context.SaveChangesAsync() > 0;
    }
    public async Task<RefreshToken> GetRefreshTokenByUserIDAsync(int userID)
    {
        RefreshToken refreshToken = await context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.UserID == userID);
        return refreshToken;
    }
    public async Task<bool> RevokeTokenAsync(int userID)
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
    public async Task<bool> UpdateWithoutPasswordAsync(string Name, string OldEmail, string Email)
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

    public async Task<User> FindByEmailAsync(string email)
    {
        var user = await context.Users
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
        if (user == null) return null;
        return user;
    }

    public async Task<bool> ConfirmEmailAsync(string email, bool IsEmailConfirmed)
    {
        var user = await context.Users
            .Where(u => u.Email == email).FirstOrDefaultAsync();
        if (user == null) return false;
        user.IsEmailConfirmed = IsEmailConfirmed;
        var userDto = mapper.Map<User>(user);
        context.Users.Update(userDto);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<PaginatedList<LogResponse>> GetLogsPaginatedAsync(int page, int pageSize, LogType logType, int userID)
    {
        var query = context.AuditLogs
            .Where(x => x.UserID == userID)
            .OrderByDescending(x => x.Timestamp)
            .AsQueryable();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Where(x => logType == LogType.All || x.Action == logType.ToString())
            .Select(x => new
            {
                x.ID,
                x.Action,
                x.TableName,
                x.OldValues,
                x.NewValues,
                x.Timestamp,
                x.PrimaryKey
            })
            .ToListAsync();

        var logResponses = items.Select(x => new LogResponse
        {
            ID = x.ID,
            Action = Enum.Parse<LogType>(x.Action),
            TableName = x.TableName,
            OldValues = x.OldValues,
            NewValues = x.NewValues,
            Timestamp = x.Timestamp,
            PrimaryKey = x.PrimaryKey
        }).ToList();

        var totalCount = items.Count;
        var paginatedList = new PaginatedList<LogResponse>(logResponses, totalCount, page, pageSize);
        return paginatedList;
    }

    public async Task<List<string>> GetUserRolesAsync(int userID)
    {
        var roles = await context.UserRoles
            .Where(x => x.UserID == userID)
            .Select(x => x.Role.Name)
            .ToListAsync();
        return roles;
    }

    public async Task<bool> CreateUserRoleAsync(UserRole userRole, bool saveChanges = true)
    {
        context.UserRoles.Add(userRole);
        if (saveChanges)
            return await context.SaveChangesAsync() > 0;

        return true;
    }

    public async Task<PaginatedList<UserPaginationResponse>> GetPaginatedAsync(int page, int pageSize)
    {
        var query = context.Users
            .OrderByDescending(x => x.CreatedAt)
            .AsQueryable();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new UserPaginationResponse
            {
                ID = x.ID,
                Name = x.Name,
                Email = x.Email,
                IsEmailConfirmed = x.IsEmailConfirmed,
                Wallets = x.UserWallets.Select(uw => new UserWalletPaginationResponse
                {
                    WalletID = uw.WalletID,
                    Role = uw.Role,
                    Balance = uw.Wallet.Balance,
                }).ToList(),
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();
        var totalCount = await query.CountAsync();
        var paginatedList = new PaginatedList<UserPaginationResponse>(items, totalCount, page, pageSize);
        return paginatedList;
    }
}