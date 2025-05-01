using AutoMapper;
using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Mappers;
public class EntryMapper : Profile
{
    public EntryMapper()
    {
        CreateMap<EntryResponse, Entry>();
        CreateMap<Entry, EntryResponse>();
        CreateMap<Entry, EntryDto>();
    }
}
