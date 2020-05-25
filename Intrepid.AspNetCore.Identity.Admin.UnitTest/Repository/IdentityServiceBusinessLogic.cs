using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.BusinessLogic;
using Intrepid.AspNetCore.Identity.Admin.Common.Extensions;
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Intrepid.AspNetCore.Identity.Admin.EntityFramework.Shared.DbContexts;
using Intrepid.AspNetCore.Identity.Admin.EntityFramework.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Intrepid.AspNetCore.Identity.Admin.UnitTest.Repository
{

    public class IdentityServiceBusinessLogic : TestBaseClass
    {
        public IdentityServiceBusinessLogic(DatabaseFixture fixture) : base(fixture) { }
        [Fact]
        public async Task InsertIdentityUserWithRolesTest()
        {
            var manager = this.Fixture.Provider.GetRequiredService<UserManager<ApplicationIdentityUser>>();
            var dbContext = this.Fixture.Provider.GetRequiredService<ApplicationDbContext>();
            dbContext.Roles.Add(new ApplicationIdentityRole { 
                Id=Guid.NewGuid().ToString(),
                Name= "InsertIdentityUserWithRolesTest",
                NormalizedName= "InsertIdentityUserWithRolesTest".ToUpper()
            });
            await dbContext.SaveChangesAsync();
            var identityservice = new IdentityService(this.Fixture.Provider.GetRequiredService<UserManager<ApplicationIdentityUser>>(),
                this.Fixture.Provider.GetRequiredService<ApplicationDbContext>(),
                this.Fixture.Provider.GetRequiredService<IMapper>(), this.Fixture.Provider.GetRequiredService<ILogger<IdentityService>>());
            var emailGenerated = Guid.NewGuid() + "@hotmail.com";
            var password = "Password123";
            var requireCreatedIdentityUser = new IdentityUserDTO()
            {
                Id = Guid.NewGuid().ToString(),
                Email = emailGenerated,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                UserName = emailGenerated,
                Roles=new List<string> { "InsertIdentityUserWithRolesTest" }
            };

            var result = await identityservice.CreateUser(requireCreatedIdentityUser, password);
            Assert.True(result.IsSuccess, "create user success failure");
            Assert.True(result.ReturnObject.Roles.First(x=>x.Contains("InsertIdentityUserWithRolesTest"))!=null, "Cannot fine role");

        }
        [Fact]
        public async Task<bool> InsertIdentityUserTest()
        {
            var manager = this.Fixture.Provider.GetRequiredService<UserManager<ApplicationIdentityUser>>();
            var identityservice = new IdentityService(this.Fixture.Provider.GetRequiredService<UserManager<ApplicationIdentityUser>>(),
                this.Fixture.Provider.GetRequiredService<ApplicationDbContext>(),
                this.Fixture.Provider.GetRequiredService<IMapper>(), this.Fixture.Provider.GetRequiredService<ILogger<IdentityService>>());
            var emailGenerated = Guid.NewGuid() + "@hotmail.com";
            var password = "password123";
            var requireCreatedIdentityUser = new IdentityUserDTO()
            {
                Id = Guid.NewGuid().ToString(),
                Email = emailGenerated,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                UserName = emailGenerated,

            };

            var result = await identityservice.CreateUser(requireCreatedIdentityUser, password);
            Assert.True(!result.IsSuccess, "create user success failure");
            Assert.True(result.IdentityError.Count == 1 && result.IdentityError.First(x => x.Code.Contains("PasswordRequiresUpper"))!=null, "create user success failure");
            //this is rather a dummy test, no point to test if password requirement is needed since it is identity implmenetaiton, this just demonstrate the unit testing purpose
            password = "passworD123";
            result = await identityservice.CreateUser(requireCreatedIdentityUser, password);
            Assert.True(result.IsSuccess, "create user success fail");
            Assert.True(result.IsSuccess && !string.IsNullOrEmpty(result.ReturnObject.SecurityStamp), "create user success fail");
            //another one, this is comparing password hasher
            var identityuser = await manager.FindByIdAsync(requireCreatedIdentityUser.Id);
            var passwordVerifyHashResult = manager.PasswordHasher.VerifyHashedPassword(identityuser, identityuser.PasswordHash, password);
            Assert.True(passwordVerifyHashResult == PasswordVerificationResult.Success, "hash incorrectly");
            passwordVerifyHashResult = manager.PasswordHasher.VerifyHashedPassword(identityuser, identityuser.PasswordHash, password + "1");
            Assert.True(passwordVerifyHashResult == PasswordVerificationResult.Failed, "hash comparison should have failed");


            return true;
        }
        [Fact]
        public async Task<bool> VerifyUserPassword()
        {
            var manager = this.Fixture.Provider.GetRequiredService<UserManager<ApplicationIdentityUser>>();
            var identityservice = new IdentityService(this.Fixture.Provider.GetRequiredService<UserManager<ApplicationIdentityUser>>(),
                this.Fixture.Provider.GetRequiredService<ApplicationDbContext>(),
                this.Fixture.Provider.GetRequiredService<IMapper>(), this.Fixture.Provider.GetRequiredService<ILogger<IdentityService>>());
            var emailGenerated = Guid.NewGuid() + "@hotmail.com";
            var password = "Password123";
            var requireCreatedIdentityUser = new IdentityUserDTO()
            {
                Id = Guid.NewGuid().ToString(),
                Email = emailGenerated,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                UserName = emailGenerated,

            };
            var result = await identityservice.CreateUser(requireCreatedIdentityUser, password);
            Assert.True(result.IsSuccess, "Failed to create dummy account");
            var resultChangePassword = await identityservice.VerifyUserPassword(requireCreatedIdentityUser.Id, password + "1");
            Assert.False(resultChangePassword.IsSuccess, "incorrect result, incorrect password");
            var user = await manager.FindByIdAsync(requireCreatedIdentityUser.Id);
            Assert.True(user.AccessFailedCount == 1, "forgot to update the counter");

            resultChangePassword = await identityservice.VerifyUserPassword(requireCreatedIdentityUser.Id, password);
            Assert.True(resultChangePassword.IsSuccess, "incorrect result, incorrect password");
            user = await manager.FindByIdAsync(requireCreatedIdentityUser.Id);
            Assert.True(user.AccessFailedCount == 0, "forgot to reset the counter");
            return true;
        }
        [Fact]
        public async Task<bool> ChangeUserPassword()
        {
            var manager = this.Fixture.Provider.GetRequiredService<UserManager<ApplicationIdentityUser>>();
            var identityservice = new IdentityService(this.Fixture.Provider.GetRequiredService<UserManager<ApplicationIdentityUser>>(),
                this.Fixture.Provider.GetRequiredService<ApplicationDbContext>(),
                this.Fixture.Provider.GetRequiredService<IMapper>(), this.Fixture.Provider.GetRequiredService<ILogger<IdentityService>>());
            var emailGenerated = Guid.NewGuid() + "@hotmail.com";
            var password = "Password123";
            var requireCreatedIdentityUser = new IdentityUserDTO()
            {
                Id = Guid.NewGuid().ToString(),
                Email = emailGenerated,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                UserName = emailGenerated,
            };
            var result = await identityservice.CreateUser(requireCreatedIdentityUser, password);
            Assert.True(result.IsSuccess, "create user success failure");
            Assert.True(result.ReturnObject.AccessFailedCount == 0, "create user success failure");
            var resultChangePassword = await identityservice.ChangeUserPassword(requireCreatedIdentityUser.Id, password + "1", password + "2");
            Assert.False(resultChangePassword.IsSuccess, "change password should have failed");
            var user = await manager.FindByIdAsync(requireCreatedIdentityUser.Id);
            Assert.True(user.AccessFailedCount == 1, "should have increment after incorrect password");
            resultChangePassword = await identityservice.ChangeUserPassword(requireCreatedIdentityUser.Id, password, password + "2");
            Assert.True(resultChangePassword.IsSuccess, "update password failed");
            user = await manager.FindByIdAsync(requireCreatedIdentityUser.Id);
            Assert.True(user.AccessFailedCount == 0, "should have reset count after password reset");
            return true;
        }
        [Fact]
        public async Task UpdateIdentityUser()
        {
            var manager = this.Fixture.Provider.GetRequiredService<UserManager<ApplicationIdentityUser>>();
            var mapper = this.Fixture.Provider.GetRequiredService<IMapper>();
            var dbContext = this.Fixture.Provider.GetRequiredService<ApplicationDbContext>();
            dbContext.Roles.Add(new ApplicationIdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "UpdateIdentityUser1",
                NormalizedName = "UpdateIdentityUser1".ToUpper()
            });
            dbContext.Roles.Add(new ApplicationIdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "UpdateIdentityUser2",
                NormalizedName = "UpdateIdentityUser2".ToUpper()
            });
            await dbContext.SaveChangesAsync();
            var identityservice = new IdentityService(this.Fixture.Provider.GetRequiredService<UserManager<ApplicationIdentityUser>>(),
                this.Fixture.Provider.GetRequiredService<ApplicationDbContext>(),
                this.Fixture.Provider.GetRequiredService<IMapper>(), this.Fixture.Provider.GetRequiredService<ILogger<IdentityService>>());
            var emailGenerated = Guid.NewGuid() + "@hotmail.com";
            var password = "Password123";
            var requireCreatedIdentityUser = new IdentityUserDTO()
            {
                Id = Guid.NewGuid().ToString(),
                Email = emailGenerated,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                UserName = emailGenerated,
                Roles = new List<string> { "UpdateIdentityUser2" }
            };
            dbContext.DetachAllEntities();
            var result = await identityservice.CreateUser(requireCreatedIdentityUser, password);
            Assert.True(result.IsSuccess, "create user success failure");
            Assert.True(result.ReturnObject.Roles.First(x => x.Contains("UpdateIdentityUser2")) != null, "Cannot fine role");
            //ok update htat user
            dbContext.DetachAllEntities();
            var updateuserDTO = result.ReturnObject;
            updateuserDTO.Roles.Clear();
            updateuserDTO.Roles.Add("UpdateIdentityUser1");
            updateuserDTO.PasswordHash = string.Empty;
            updateuserDTO.PhoneNumber = "12345";
            result = await identityservice.UpdateUser(updateuserDTO);

            Assert.True(result.IsSuccess, "update user success failure");
            Assert.True(result.ReturnObject.Roles.First(x => x.Contains("UpdateIdentityUser1")) != null, "Cannot fine role");

            updateuserDTO = result.ReturnObject;
            updateuserDTO.PasswordHash = string.Empty;
            updateuserDTO.Roles.Clear();
            updateuserDTO.Roles.Add(Guid.NewGuid().ToString());
            result = await identityservice.UpdateUser(updateuserDTO);
            Assert.False(result.IsSuccess, "update success expecting failure");
        }
    }
}

