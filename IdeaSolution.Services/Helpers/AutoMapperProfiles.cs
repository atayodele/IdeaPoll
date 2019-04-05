using AutoMapper;
using IdeaSolution.Data.Models;
using IdeaSolution.Services.Dto;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace IdeaSolution.Services.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RegisterForDto, AppUser>();
            CreateMap<AppUser, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt =>
                {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                });            
            CreateMap<IdentityRole, RoleForListDto>();
            CreateMap<RoleForDto, IdentityRole>();
            CreateMap<RoleForUpdateDto, IdentityRole>();
            CreateMap<AppUser, UserRoleListDto>()
                .ForMember(dest => dest.Age, opt =>
                {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                });
            CreateMap<AppUser, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt =>
                {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                });
            CreateMap<UserForUpdateDto, AppUser>();
        }
    }
}
