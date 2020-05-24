using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Intrepid.AspNetCore.Identity.Admin.UnitTest
{
    [Collection("Database collection")]
    public abstract class TestBaseClass
    {
        public DatabaseFixture Fixture { get; }
        public TestBaseClass(DatabaseFixture fixture)
        {
            this.Fixture = fixture;
        }

        
    }
}
