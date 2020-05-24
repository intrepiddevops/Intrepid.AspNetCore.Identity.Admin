using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.BusinessLogic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Intrepid.AspNetCore.Identity.Admin.UnitTest
{
    
    public class GenericTest:TestBaseClass
    {
        public GenericTest(DatabaseFixture fixture):base(fixture)
        {
        }
        [Fact]
        public async Task<bool> TestFixure()
        {
            Assert.True(true, "Incorrect Setup");
            return true;
        }
    }
}
