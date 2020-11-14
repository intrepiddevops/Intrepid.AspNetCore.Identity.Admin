using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Intrepid.AspNetCore.Identity.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Configurations.Mapper
{
    public class IdentityUserDTO2UserForCreationModel : Profile
    {
        public IdentityUserDTO2UserForCreationModel()
        {
            CreateMap<IdentityUserDTO, UserForCreationModel>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(source => source.PasswordHash))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(source => source.PhoneNumber))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(source => source.Email))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(source => source.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(source => source.UserName))
                .ForMember(dest => dest.ConconcurrenyStamp, opt => opt.MapFrom(source => source.ConcurrencyStamp))
                .ForMember(dest=>dest.Roles, opt=>opt.MapFrom(source=>source.Roles))
                .ReverseMap();

        }
    }
}
