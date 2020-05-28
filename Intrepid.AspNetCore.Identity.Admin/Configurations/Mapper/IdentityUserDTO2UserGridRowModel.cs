using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Intrepid.AspNetCore.Identity.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Configurations.Mapper
{
    public class IdentityUserDTO2UserGridRowModel : Profile
    {
        public IdentityUserDTO2UserGridRowModel()
        {
            CreateMap<IdentityUserDTO, UserGridRowModel>()
               .ForMember(x => x.UserId, opt => opt.MapFrom(source => source.Id))
               .ForMember(x => x.Username, opt => opt.MapFrom(role => role.UserName))
               .ForMember(x => x.Phone, opt => opt.MapFrom(role => role.PhoneNumber))
               .ForMember(x => x.IsTwoFactorEnabled, opt => opt.MapFrom(role => role.TwoFactorEnabled))
               .ForMember(x => x.IsLocked, opt => opt.MapFrom(role => role.LockoutEnabled && DateTime.UtcNow.CompareTo(role.LockoutEnd)<=0))
               .ForMember(x => x.Email, opt => opt.MapFrom(role => role.Email));
        }
    }
}
