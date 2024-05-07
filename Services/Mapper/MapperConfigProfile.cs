using AutoMapper;
using Repositories.Entities;
using Services.ViewModels.AccountModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Repositories.DTO;

namespace Services.Mapper
{
    public class MapperConfigProfile : Profile
    {
        public MapperConfigProfile() 
        {
            CreateMap<AccountDetailsModel,Account>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
           .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToLower() == "male"))
           // Chuyển đổi Guid sang string
           .ReverseMap();
        }
    }
}
