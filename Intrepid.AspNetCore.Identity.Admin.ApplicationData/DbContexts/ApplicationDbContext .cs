using Intrepid.AspNetCore.Identity.Admin.ApplicationData;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intrepid.AspNetCore.Identity.Admin.Database.Shared.DbContexts
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
