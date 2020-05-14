using AutoMapper;
using AutoMapper.QueryableExtensions;
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.BusinessLogic
{
    public class IdentityService: BaseClass
    {
        public IdentityService(IdentityDbContext context, IMapper mapper, ILogger<IdentityService> logger):base(context, mapper, logger)
        {

        }
        public async Task<List<IdentityUserDTO>> SearchUser(string email, string name)
        {
            var query = this.Context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(email))
                query = query.Where(x => EF.Functions.Like(x.NormalizedEmail, email));
            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => EF.Functions.Like(x.NormalizedUserName, name));
            return await query.ProjectTo<IdentityUserDTO>(this.Mapper.ConfigurationProvider).ToListAsync();
        }
    }
}
