using AutoMapper;
using WorkingMVC.Data.Entities;
using WorkingMVC.Models.Category;

namespace WorkingMVC.Mappers;

public class CategoryMapper : Profile
{
    public CategoryMapper()
    {
        CreateMap<CategoryEntity, CategoryItemModel>();
        CreateMap<CategoryEntity, CategoryUpdateModel>()
            .ForMember(dest => dest.CurrentImage, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Image, opt => opt.Ignore());

        CreateMap<CategoryUpdateModel, CategoryEntity>()
            .ForMember(dest => dest.Image, opt => opt.Ignore());
    }
}
