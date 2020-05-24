using AutoMapper;
using AutoMapper.QueryableExtensions;
using Intrepid.AspNetCore.Identity.Admin.Common.Extensions;
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.XPath;

namespace Intrepid.AspNetCore.Identity.Admin.BusinessLogic
{
    public class IdentityService: BaseClass
    {
        public UserManager<IdentityUser> Manager { get; private set; }
        public IdentityService(UserManager<IdentityUser> manager, IdentityDbContext dbContext, IMapper mapper, ILogger<IdentityService> logger):base(dbContext, mapper, logger)
        {
            Manager = manager;
        }


        public async Task<ResultDTO<bool>> VerifyUserPassword(string userId, string password)
        {
            // i think in thoery we don't need to have this one, the only difference checkpassword also increment
            var result = new ResultDTO<bool>() { IsSuccess = false };
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await this.Manager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        if (!await this.Manager.CheckPasswordAsync(user, password))
                        {

                            await this.Manager.AccessFailedAsync(user);
                            
                        }
                        else
                        {
                            await this.Manager.ResetAccessFailedCountAsync(user);
                            result.IsSuccess = true;
                        }
                    }
                    else
                    {
                        result.ErrorMsg.Add("incorrect user or password");
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    result.ErrorMsg.Add("generic exception");
                    result.GenericException = ex;
                    await transaction.RollbackAsync();
                }
            }
            return result;      
        }

        public async Task<List<IdentityUserDTO>> SearchUser(string email, string name)
        {
            var query = Manager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(email))
                query = query.Where(x => EF.Functions.Like(x.NormalizedEmail, email));
            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => EF.Functions.Like(x.NormalizedUserName, name));
            return await query.ProjectTo<IdentityUserDTO>(this.Mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<IdentityUserMetaData> UsersMetaData()
        {
            var query = Manager.Users.AsQueryable();
            var currentDateTime = DateTime.UtcNow;
            var countQuery = Manager.Users.AsQueryable().Select(e => new {
                Locked = (e.LockoutEnabled && e.LockoutEnd.HasValue && e.LockoutEnd.Value > currentDateTime) ? 1 : 0,
                EmailNotConfired = (e.EmailConfirmed ? 0 : 1)

            }).GroupBy(e=>1)
            .Select(g => new {
                Count=g.Count(),
                LockedCount=g.Sum(e=>e.Locked),
                EmailNotConfiredCount=g.Sum(e=>e.EmailNotConfired)
            });
            var user=await this.Manager.FindByNameAsync("user@intrepiddevops.com");
            var result = await countQuery.FirstOrDefaultAsync();
            return new IdentityUserMetaData { TotalNumberUsers=result.Count, TotalEmailNotConfirm = result.LockedCount, TotlaLockedOut = result.LockedCount};
        }


        public async Task<ResultDTO<IdentityUserDTO>> CreateUser(IdentityUserDTO userDto, string password)
        {
            var resultDto = new ResultDTO<IdentityUserDTO>();
            resultDto.IsSuccess = false;
            using (var transaction = await this.Context.Database.BeginTransactionAsync())
            {
                try
                {

                    var user = this.Mapper.Map<IdentityUser>(userDto);
                    if (!Manager.SupportsQueryableUsers)
                        throw new Exception("Does not support Queryable Users");
                    var result = await Manager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        var createdUser = await this.Manager.FindByIdAsync(user.Id);
                        //fetch by the new created user
                        resultDto.ReturnObject = this.Mapper.Map<IdentityUserDTO>(createdUser);
                        //before we completed, if there is new role mapping

                        var roleResult = await this.Manager.AddToRolesAsync(createdUser, userDto.Roles);
                        if (!roleResult.Succeeded)
                        {
                            resultDto.IdentityError = this.Mapper.Map<List<IdentityErrorDTO>>(result.Errors.ToList());
                            throw new Exception("Role creation failed");
                        }
                        

                        resultDto.IsSuccess = true;
                    }
                    else
                    {
                        resultDto.IdentityError = this.Mapper.Map<List<IdentityErrorDTO>>(result.Errors.ToList());
                    }
                    transaction.Commit();
                    //at this point get the user one more time, to ensure
                    if (resultDto.IsSuccess)
                    {
                        var createdUser2 = await this.Manager.FindByIdAsync(user.Id);
                        resultDto.ReturnObject = this.Mapper.Map<IdentityUserDTO>(createdUser2);
                        resultDto.ReturnObject.Roles = (await this.Manager.GetRolesAsync(createdUser2)).ToList();
                    }
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    resultDto.DetailErrorException = ex.Message;
                    resultDto.ErrorMsg.Add("DB Update Failure");
                    Logger.LogError(ex, "SQL Update Exception");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    resultDto.DetailErrorException = ex.Message;
                    resultDto.ErrorMsg.Add("Generic Failure");
                    Logger.LogError(ex, "Generic Exception");
                }
            }
            return resultDto;
        }

        public async Task<ResultDTO<bool>> ChangeUserPassword(string userId, string currentPassword, string newPassword)
        {
            var resultDto = new ResultDTO<bool>
            {
                IsSuccess = false
            };
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                try
                {

                    var identityUser = await this.Manager.FindByIdAsync(userId);
                    if (identityUser == null)
                    {
                        resultDto.ErrorMsg.Add("Incorrect username or password");
                        return resultDto;
                    }
                    var result = await this.Manager.ChangePasswordAsync(identityUser, currentPassword, newPassword);
                    resultDto.IsSuccess = result.Succeeded;
                    if (!result.Succeeded)
                    {
                        await this.Manager.AccessFailedAsync(identityUser);
                        resultDto.IdentityError = this.Mapper.Map<List<IdentityErrorDTO>>(result.Errors.ToList());
                    }
                    else
                    {
                        //ok completed, need to reset the sync
                        await this.Manager.ResetAccessFailedCountAsync(identityUser);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    resultDto.DetailErrorException = ex.Message;
                    resultDto.GenericException = ex;
                }
            }
            
            return resultDto;
        }

        public async Task<ResultDTO<IdentityUserDTO>> UpdateUser(IdentityUserDTO userDto)
        {
            var resultDTO = new ResultDTO<IdentityUserDTO>();
            if (!string.IsNullOrEmpty(userDto.PasswordHash))
            {
                resultDTO.ErrorMsg.Add("Cannot update password hash");
                return resultDTO;
            }
            using (var transaction = await this.Context.Database.BeginTransactionAsync())
            {
                try
                {
                    //mapto identity user
                    var updatedUserIncomgin = await this.Manager.FindByIdAsync(userDto.Id);
                    

                    //remove all the roles
                    if (updatedUserIncomgin == null || updatedUserIncomgin.ConcurrencyStamp != userDto.ConcurrencyStamp)
                    {
                        resultDTO.ErrorMsg.Add("Cannot find user");
                        return resultDTO;
                    }
                    
                    var existingRoles = await this.Manager.GetRolesAsync(updatedUserIncomgin);
                    
                    var identityResult = await this.Manager.RemoveFromRolesAsync(updatedUserIncomgin, existingRoles);
                    if (!identityResult.Succeeded)
                    {
                        resultDTO.IdentityError = this.Mapper.Map<List<IdentityErrorDTO>>(identityResult.Errors);
                        return resultDTO;
                    }

                    //ok all done, mapping the incoming to the new identity user
                    //mapping the changes
                    updatedUserIncomgin.Email = userDto.Email;
                    updatedUserIncomgin.EmailConfirmed = userDto.EmailConfirmed;
                    updatedUserIncomgin.NormalizedEmail = userDto.Email.ToUpper();
                    updatedUserIncomgin.UserName = userDto.UserName;
                    updatedUserIncomgin.NormalizedUserName = userDto.UserName.ToUpper();
                    updatedUserIncomgin.PhoneNumber = userDto.PhoneNumber;
                    updatedUserIncomgin.PhoneNumberConfirmed = userDto.PhoneNumberConfirmed;
                    updatedUserIncomgin.TwoFactorEnabled = userDto.TwoFactorEnabled;
                    



                    identityResult = await this.Manager.UpdateAsync(updatedUserIncomgin);
                    if (!identityResult.Succeeded)
                    {
                        resultDTO.IdentityError = this.Mapper.Map<List<IdentityErrorDTO>>(identityResult.Errors);
                        return resultDTO;
                    }
                    //repopulate the role

                    identityResult = await this.Manager.AddToRolesAsync(updatedUserIncomgin, userDto.Roles);
                    if (!identityResult.Succeeded)
                    {
                        resultDTO.IdentityError = this.Mapper.Map<List<IdentityErrorDTO>>(identityResult.Errors);
                        return resultDTO;
                    }
                    resultDTO.IsSuccess = true;
                    transaction.Commit();

                    if (resultDTO.IsSuccess)
                    {
                        var createdUser2 = await this.Manager.FindByIdAsync(userDto.Id);
                        resultDTO.ReturnObject = this.Mapper.Map<IdentityUserDTO>(createdUser2);
                        resultDTO.ReturnObject.Roles = (await this.Manager.GetRolesAsync(createdUser2)).ToList();
                    }
                    return resultDTO;

                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Generic failure");
                    resultDTO.GenericException = ex;
                }
            }
            //ok all done
            return resultDTO;
        }
    }
}
