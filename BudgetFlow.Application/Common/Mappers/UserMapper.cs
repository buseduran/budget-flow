using AutoMapper;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.User;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Mappers;
public class UserMapper : Profile
{
    protected UserMapper()
    {
        CreateMap<UserModel, UserDto>().ReverseMap();
        CreateMap<UserResponse, UserDto>().ReverseMap();
    }
}
