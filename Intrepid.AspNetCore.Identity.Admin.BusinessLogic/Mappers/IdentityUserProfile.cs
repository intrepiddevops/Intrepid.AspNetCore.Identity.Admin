using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intrepid.AspNetCore.Identity.Admin.BusinessLogic.Mappers
{
    public class IdentityUserProfile : Profile
    {
        public IdentityUserProfile()
        {
            CreateMap<IdentityUser, IdentityUserDTO>().ForMember(x => x.Roles, opt => opt.MapFrom(u=>new List<string>()));
            CreateMap<IdentityUserDTO, IdentityUser>();
        }
    }
}
