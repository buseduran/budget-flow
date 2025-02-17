using AutoMapper;
using BudgetFlow.Application.Budget;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Mappers
{
    public class EntryMapper : Profile
    {
        public EntryMapper()
        {
            CreateMap<EntryResponse, EntryDto>();
            CreateMap<EntryDto, EntryResponse>();
        }
    }
}
