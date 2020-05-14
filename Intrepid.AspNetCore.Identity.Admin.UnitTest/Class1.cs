using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.BusinessLogic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.UnitTest
{
    [Collection("Database collection")]
    public class IdentityServiceTest
    {
        DatabaseFixture fixture;
        public IdentityServiceTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }
        [Fact]
        public async Task<bool> Test1()
        {
            
            var service = new IdentityService(this.fixture.Provider.GetService<IdentityDbContext>(),
                this.fixture.Provider.GetService<IMapper>(), this.fixture.Provider.GetService<ILogger<IdentityService>>()) ;
            var result=await service.SearchUser(string.Empty, "steve");
            Assert.True(result.Count==0, "funfun");
            return true;
        }
    }
}
