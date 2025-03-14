using AutoMapper;
using BudgetFlow.Application.Assets;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Mappers
{
    public class AssetMapper : Profile
    {
        public AssetMapper()
        {
            CreateMap<AssetResponse, Asset>().ReverseMap();
        }
    }
}
