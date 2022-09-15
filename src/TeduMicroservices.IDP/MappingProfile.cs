using AutoMapper;
using TeduMicroservices.IDP.Infrastructure.Entities;
using TeduMicroservices.IDP.Infrastructure.ViewModels;

namespace TeduMicroservices.IDP;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Permission, PermissionViewModel>();
        CreateMap<Permission, PermissionUserViewModel>();
    }
}