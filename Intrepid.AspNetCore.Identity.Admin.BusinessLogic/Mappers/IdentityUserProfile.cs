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
            CreateMap<ApplicationIdentityUser, IdentityUserDTO>()
                .ForMember(x => x.Roles, opt => opt.MapFrom(u=>new List<string>()))
                .ForMember(x => x.AccessFailedCount, opt => opt.MapFrom(source => source.AccessFailedCount))
                .ForMember(x => x.ConcurrencyStamp, opt => opt.MapFrom(source => source.ConcurrencyStamp))
                .ForMember(x => x.Email, opt => opt.MapFrom(source => source.Email))
                .ForMember(x => x.EmailConfirmed, opt => opt.MapFrom(source => source.EmailConfirmed))
                .ForMember(x=>x.Id, opt=>opt.MapFrom(source=>source.Id))
                .ForMember(x => x.LockoutEnabled, opt => opt.MapFrom(source => source.LockoutEnabled))
                .ForMember(x => x.LockoutEnd, opt => opt.MapFrom(source => source.LockoutEnd))
                .ForMember(x => x.NormalizedEmail, opt => opt.MapFrom(source => source.NormalizedEmail))
                .ForMember(x => x.NormalizedUserName, opt => opt.MapFrom(source => source.NormalizedUserName))
                .ForMember(x => x.PasswordHash, opt => opt.MapFrom(source => source.PasswordHash))
                .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(source => source.PhoneNumber))
                .ForMember(x => x.PhoneNumberConfirmed, opt => opt.MapFrom(source => source.PhoneNumberConfirmed))
                .ForMember(x => x.SecurityStamp, opt => opt.MapFrom(source => source.SecurityStamp))
                .ForMember(x => x.TwoFactorEnabled, opt => opt.MapFrom(source => source.TwoFactorEnabled))
                .ForMember(x => x.UserName, opt => opt.MapFrom(source => source.UserName))
                .ReverseMap();
            
        }
    }
}
