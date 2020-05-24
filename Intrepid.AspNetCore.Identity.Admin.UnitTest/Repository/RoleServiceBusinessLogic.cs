using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.BusinessLogic;
using Intrepid.AspNetCore.Identity.Admin.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Intrepid.AspNetCore.Identity.Admin.UnitTest.Repository
{
    public class RoleServiceBusinessLogic : TestBaseClass
    {
        public RoleServiceBusinessLogic(DatabaseFixture fixture) : base(fixture) { }
        [Fact]
        public async Task<bool> InsertIdentityRoleTest()
        {
            var manager = this.Fixture.Provider.GetRequiredService<UserManager<IdentityUser>>();
            var roleService = new RoleService(this.Fixture.Provider.GetRequiredService<RoleManager<IdentityRole>>(),
                this.Fixture.Provider.GetRequiredService<IdentityDbContext>(),
                this.Fixture.Provider.GetRequiredService<IMapper>(), this.Fixture.Provider.GetRequiredService<ILogger<RoleService>>());
            var identityRoleDto = new IdentityRoleDTO() { 
               
                ClaimType =string.Empty,
                ClaimValue=string.Empty,
                Id=Guid.NewGuid().ToString(),
                Name="TestCreate"
            };

            var result=await roleService.CreateUpdateRole(identityRoleDto);
            Assert.True(result.IsSuccess && result.IdentityError.Count == 0 && result.ErrorMsg.Count == 0 && result.ReturnObject.Id == identityRoleDto.Id, "insertion failed");
            return true;
        }
        [Fact]
        public async Task<bool> UpdateIdentityRoleTest()
        {
            var manager = this.Fixture.Provider.GetRequiredService<UserManager<IdentityUser>>();
            var roleService = new RoleService(this.Fixture.Provider.GetRequiredService<RoleManager<IdentityRole>>(),
                this.Fixture.Provider.GetRequiredService<IdentityDbContext>(),
                this.Fixture.Provider.GetRequiredService<IMapper>(), this.Fixture.Provider.GetRequiredService<ILogger<RoleService>>());
            var identityRoleDto = new IdentityRoleDTO()
            {

                ClaimType = string.Empty,
                ClaimValue = string.Empty,
                Id = Guid.NewGuid().ToString(),
                Name = "TestUpdate"
            };

            var result = await roleService.CreateUpdateRole(identityRoleDto);
            Assert.True(result.IsSuccess && result.IdentityError.Count == 0 && result.ErrorMsg.Count == 0 && result.ReturnObject.Id == identityRoleDto.Id, "insertion failed");
            identityRoleDto.Name = "happyTime";
            identityRoleDto.ClaimType = "123";
            result = await roleService.CreateUpdateRole(identityRoleDto);
            Assert.True(result.IsSuccess && result.IdentityError.Count == 0 && result.ErrorMsg.Count == 0 && result.ReturnObject.Id == identityRoleDto.Id &&
                string.Compare(result.ReturnObject.Name, identityRoleDto.Name)==0, "update failed");
            Assert.True(result.ReturnObject.NormalizedName == identityRoleDto.Name.ToUpper(), "normalization failed");
            return true;
        }
    }
}
