using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Intrepid.AspNetCore.Identity.Admin.EntityFramework.Shared.DbContexts;
using Intrepid.AspNetCore.Identity.Admin.EntityFramework.Shared.Entities;
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
        public RoleManager<ApplicationIdentityRole> Role { get; }
        public RoleService(RoleManager<ApplicationIdentityRole> role, ApplicationDbContext dbContext, IMapper mapper, ILogger<RoleService> logger) : base(dbContext, mapper, logger)
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

        public async Task<ResultDTO<List<IdentityRoleDTO>>> GetRole(string roleid)
        {

            var result = new ResultDTO<List<IdentityRoleDTO>>();
            var aggregateCount = await (from role in this.Context.Roles
                                        where role.Id==roleid
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

            //ok populate the claim
            foreach(var role in aggregateCount)
            {
                var existingRole = await this.Role.FindByIdAsync(role.Id);
                //populate the claim
                role.Claims = (await this.Role.GetClaimsAsync(existingRole)).Select(x => x.Type + ";" + x.Value).ToList();
            }
            result.IsSuccess = true;
            result.ReturnObject = aggregateCount;
            return result;
        }
        public async Task<ResultDTO<IdentityRoleDTO>> DeleteRole(string roleId)
        {
            var resultDto = new ResultDTO<IdentityRoleDTO> { IsSuccess = false };
            var deleterole = await this.Role.FindByIdAsync(roleId);

            var deleteResult=await this.Role.DeleteAsync(deleterole);

            if (deleteResult.Succeeded)
            {
                resultDto.IsSuccess = true;
                //find the role
                resultDto.ReturnObject = null;
            }
            else
            {
                resultDto.IdentityError = this.Mapper.Map<List<IdentityErrorDTO>>(deleteResult.Errors.ToList());
            }
            return resultDto;
        }
        public async Task<ResultDTO<IdentityRoleDTO>> CreateUpdateRole(IdentityRoleDTO role)
        {
            var resultDto = new ResultDTO<IdentityRoleDTO> { IsSuccess = false };
            
            try
            {
                
                var findRole = await this.Role.FindByIdAsync(role.Id);
                ApplicationIdentityRole identityRole = null;
                var identityResult = new IdentityResult();
                if (findRole != null)
                {
                    findRole.Name = role.Name;
                    findRole.ConcurrencyStamp = role.ConcurrencyStamp;
                    
                    //this.Context.Entry(findRole).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                    identityResult = await this.Role.UpdateAsync(findRole);
                    identityRole= await this.Role.FindByIdAsync(findRole.Id);
                }
                else
                {
                    identityRole = this.Mapper.Map<ApplicationIdentityRole>(role);
                    identityRole.Id = Guid.NewGuid().ToString();
                    identityResult = await this.Role.CreateAsync(identityRole);
                }
                if (identityResult.Succeeded)
                {
                    
                    foreach (var existingClaim in (await this.Role.GetClaimsAsync(identityRole)))
                        await this.Role.RemoveClaimAsync(identityRole, existingClaim);
                    foreach(var newclaim in role.Claims)
                    {
                        string[] claimsplitvalue = newclaim.Split(';');
                        if (claimsplitvalue.Count() == 2)
                            await this.Role.AddClaimAsync(identityRole, new System.Security.Claims.Claim(claimsplitvalue[0], claimsplitvalue[1]));
                    }
                    resultDto.IsSuccess = true;
                    //before finish update claim

                    //find the role
                    var therole = await this.Role.FindByIdAsync(identityRole.Id) ;
                    resultDto.ReturnObject = Mapper.Map<IdentityRoleDTO>(therole);
                    //before we continue, populcate hte claim
                    resultDto.ReturnObject.Claims=(await this.Role.GetClaimsAsync(therole)).Select(x => x.Type + ";" + x.Value).ToList();
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
