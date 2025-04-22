using AutoMapper;
using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Mappers;
public class AssetMapper : Profile
{
    public AssetMapper()
    {
        CreateMap<AssetResponse, Asset>().ReverseMap();
        CreateMap<AssetDto, Asset>().ReverseMap();

        CreateMap<Asset, Asset>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}
