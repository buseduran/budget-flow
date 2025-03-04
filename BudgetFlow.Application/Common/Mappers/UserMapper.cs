using AutoMapper;
using BudgetFlow.Application.Auth;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Mappers;
public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserRegisterModel, User>().ReverseMap();
        CreateMap<UserResponse, User>().ReverseMap();
        CreateMap<EntryDto, EntryEntity>().ReverseMap();
    }
}
