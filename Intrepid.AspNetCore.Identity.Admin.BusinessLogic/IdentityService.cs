using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        
        public IdentityService(UserManager<IdentityUser> manager, IdentityDbContext dbContext, IMapper mapper, ILogger<IdentityService> logger):base(manager, dbContext, mapper, logger)
        {
        }


        public async Task<ResultDto<bool>> VerifyUserPassword(string userId, string password)
        {
            // i think in thoery we don't need to have this one, the only difference checkpassword also increment
            var result = new ResultDto<bool>() { IsSuccess = false };
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


        public async Task<ResultDto<IdentityUserDTO>> CreateUser(IdentityUserDTO userDto, string password)
        {
            var resultDto = new ResultDto<IdentityUserDTO>();
            resultDto.IsSuccess = false;
            try
            {
                var user = this.Mapper.Map<IdentityUser>(userDto);
                if (!Manager.SupportsQueryableUsers)
                    throw new Exception("Does not support Queryable Users");
                var result = await Manager.CreateAsync(user, password);
                
                if (result.Succeeded)
                {
                    //fetch by the new created user
                    resultDto.ReturnObject = this.Mapper.Map<IdentityUserDTO>(await this.Manager.FindByIdAsync(user.Id));
                    resultDto.IsSuccess = true;
                }
                else
                {

                    resultDto.ErrorMsg = result.Errors.Select(x=>$"{x.Code}:{x.Description}").ToList();
                }
            }
            catch(DbUpdateException ex)
            {
                resultDto.DetailErrorException = ex.Message;
                resultDto.ErrorMsg.Add("DB Update Failure");
                Logger.LogError(ex, "SQL Update Exception");
            }
            catch(Exception ex)
            {
                resultDto.DetailErrorException = ex.Message;
                resultDto.ErrorMsg.Add("Generic Failure");
                Logger.LogError(ex, "Generic Exception");
            }
            return resultDto;
        }

        public async Task<ResultDto<bool>> ChangeUserPassword(string userId, string currentPassword, string newPassword)
        {
            var resultDto = new ResultDto<bool>
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
                        resultDto.ErrorMsg = result.Errors.Select(x => $"{x.Code}:{x.Description}").ToList();
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
    }
}
