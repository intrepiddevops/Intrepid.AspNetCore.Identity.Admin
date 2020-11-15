using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Intrepid.AspNetCore.Identity.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Configurations.Mapper
{
    public class IdentityRoleDTOProfile : Profile
    {
        public IdentityRoleDTOProfile()
        {
            CreateMap<IdentityRoleDTO, RoleCountModel>()
                .ForMember(x => x.Name, opt => opt.MapFrom(role => role.Name))
                .ForMember(x => x.RoleId, opt => opt.MapFrom(role => role.Id))
                .ForMember(x => x.Count, opt => opt.MapFrom(role => role.UserCount))
                .ForMember(x => x.ConcurrencyStamp, opt => opt.MapFrom(role => role.ConcurrencyStamp))
                .ForMember(x => x.SelectedClaims, opt => opt.MapFrom(role => role.Claims))
                .ReverseMap();
        }
    }
}
