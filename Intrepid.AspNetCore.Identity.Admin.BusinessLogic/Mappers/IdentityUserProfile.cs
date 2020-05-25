using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Intrepid.AspNetCore.Identity.Admin.EntityFramework.Shared.Entities;
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
            CreateMap<ApplicationIdentityUser, IdentityUserDTO>().ForMember(x => x.Roles, opt => opt.MapFrom(u=>new List<string>()));
            CreateMap<IdentityUserDTO, ApplicationIdentityUser>();
        }
    }
}
