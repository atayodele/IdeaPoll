using AutoMapper;
using IdeaSolution.Data.Models;
using IdeaSolution.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdeaSolution.Services.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<RegisterForDto, AppUser>();
        }
    }
}
