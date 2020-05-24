using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.BusinessLogic
{
    public class RoleService : BaseClass
    {
        public RoleManager<IdentityRole> Role { get; }
        public RoleService(RoleManager<IdentityRole> role, IdentityDbContext dbContext, IMapper mapper, ILogger<RoleService> logger) : base(dbContext, mapper, logger)
        {
            this.Role = role;
        }


        public async Task<ResultDTO<List<IdentityRoleDTO>>> AllRoleInfo()
        {

            var result = new ResultDTO<List<IdentityRoleDTO>>();
            var aggregateCount = await (from role in this.Context.Roles
                                 let cCount =
                                 (
                                    from c in Context.UserRoles
                                    where role.Id == c.RoleId
                                    select c
                                 ).Count()
                                 select new IdentityRoleDTO()
                                 {
                                     Id = role.Id,
                                     Name = role.Name,
                                     NormalizedName = role.NormalizedName,
                                     ConcurrencyStamp = role.ConcurrencyStamp,
                                     UserCount = cCount
                                 }).ToListAsync();
            result.IsSuccess = true;
            result.ReturnObject = aggregateCount;
            return result;
        }
        public async Task<ResultDTO<IdentityRoleDTO>> CreateUpdateRole(IdentityRoleDTO role)
        {
            var resultDto = new ResultDTO<IdentityRoleDTO> { IsSuccess = false };
            var identityRole = this.Mapper.Map<IdentityRole>(role);
            try
            {
                var findRole = await this.Role.FindByIdAsync(role.Id);
                
                var identityResult = new IdentityResult();
                if (findRole!=null)
                {
                    this.Context.Entry(findRole).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                    identityResult = await this.Role.UpdateAsync(identityRole);
                }
                else
                    identityResult = await this.Role.CreateAsync(identityRole);
                if (identityResult.Succeeded)
                {
                    resultDto.IsSuccess = true;
                    //find the role
                    resultDto.ReturnObject = Mapper.Map<IdentityRoleDTO>(await this.Role.FindByIdAsync(role.Id));
                }
                else
                {
                    resultDto.IdentityError = this.Mapper.Map<List<IdentityErrorDTO>>(identityResult.Errors.ToList());
                }
            }
            catch (Exception ex)
            {
                resultDto.ErrorMsg.Add("Generic Exception occur");
                resultDto.IsSuccess = false;
                resultDto.GenericException = ex;
            }
            return resultDto;
        }
    }
}
