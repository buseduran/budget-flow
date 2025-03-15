using AutoMapper;
using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Mappers
{
    public class AssetMapper : Profile
    {
        public AssetMapper()
        {
            CreateMap<AssetResponse, Asset>().ReverseMap();
            CreateMap<AssetDto, Asset>().ReverseMap();
        }
    }
}
