using AutoMapper;
using WorkingMVC.Data.Entities.Identity;
using WorkingMVC.Models.Users;

namespace WorkingMVC.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserItemModel, EditUserViewModel>()
            .ForMember(x => x.Roles, opt => opt.MapFrom(ur => ur.Roles));
        CreateMap<UserEntity, EditUserViewModel>()
            .ForMember(x => x.Roles, opt =>
                opt.MapFrom(src => src.UserRoles != null
                    ? src.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : null).Where(n => n != null).ToArray()
                    : Array.Empty<string>()));

        CreateMap<UserEntity, UserItemModel>()
            .ForMember(x => x.FullName, opt =>
                opt.MapFrom(x => $"{x.LastName} {x.FirstName}"))
            .ForMember(x => x.Image, opt =>
                opt.MapFrom(x => string.IsNullOrEmpty(x.Image) ? "default.webp" : x.Image))
            .ForMember(x => x.Roles, opt =>
                opt.MapFrom(x => x.UserRoles != null
                    ? x.UserRoles.Select(ur => ur.Role.Name).ToArray()
                    : Array.Empty<string>()));
    }
}
