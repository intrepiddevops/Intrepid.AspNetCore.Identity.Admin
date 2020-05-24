using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intrepid.AspNetCore.Identity.Admin.BusinessLogic.Mappers
{
    public class IdentityRoleProfile : Profile
    {
        public IdentityRoleProfile()
        {
            CreateMap<IdentityRole, IdentityRoleDTO>().ForMember(x => x.UserCount, opts => opts.Ignore()).ReverseMap();
        }
    }
}
