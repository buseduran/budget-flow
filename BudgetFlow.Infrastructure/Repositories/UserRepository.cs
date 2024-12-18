using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.User;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;

namespace BudgetFlow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BudgetContext _context;
    private readonly IMapper _mapper;
    public UserRepository(BudgetContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> CreateAsync(UserModel userModel)
    {
        var user = _mapper.Map<UserDto>(userModel);
        await _context.Users.AddAsync(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int ID)
    {
        var user = await _context.Users.FindAsync(ID);
        if (user == null) return false;
        _context.Users.Remove(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<UserResponse> GetByIdAsync(int ID)
    {
        var user = await _context.Users.FindAsync(ID);
        if (user == null) return null;
        var response = _mapper.Map<UserResponse>(user);
        return response;
    }

    public async Task<bool> UpdateAsync(int ID, UserModel userModel)
    {
        var user = await _context.Users.FindAsync(ID);
        if (user == null) return false;

        var userDto = _mapper.Map<UserDto>(userModel);
        _context.Users.Update(userDto);
        return await _context.SaveChangesAsync() > 0;
    }
}

