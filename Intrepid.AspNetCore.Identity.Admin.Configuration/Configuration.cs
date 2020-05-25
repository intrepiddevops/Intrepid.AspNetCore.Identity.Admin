using Intrepid.AspNetCore.Identity.Admin.EntityFramework.Shared.DbContexts;
using Intrepid.AspNetCore.Identity.Admin.EntityFramework.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Configuration
{
    public static class Configuration
    {
        public static async Task EnsureSeedData(IHost host)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                using (var scope = serviceScope.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationIdentityUser>>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationIdentityRole>>();
                    var identityConfiguration = scope.ServiceProvider.GetRequiredService<IdentityDataConfiguration>();

                    await EnsureSeedIdentityData(identityConfiguration, roleManager, userManager);

                }
            }
        }
        private static async Task EnsureSeedIdentityData(IdentityDataConfiguration model, RoleManager<ApplicationIdentityRole> roleManager, UserManager<ApplicationIdentityUser> userManager)
        {
            if (!await roleManager.Roles.AnyAsync())
            {
                // adding roles from seed
                
                foreach (var r in model.Roles)
                {
                    if (!await roleManager.RoleExistsAsync(r.Name))
                    {
                        var role = new ApplicationIdentityRole
                        {
                            Name = r.Name
                        };

                        var result = await roleManager.CreateAsync(role);

                        if (result.Succeeded)
                        {
                            foreach (var claim in r.Claims)
                            {
                                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(claim.Type, claim.Value));
                            }
                        }
                    }
                }
            }

            if (!await userManager.Users.AnyAsync())
            {
                // adding users from seed
                foreach (var user in model.Users)
                {
                    var identityUser = new ApplicationIdentityUser
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        EmailConfirmed = true
                    };

                    // if there is no password we create user without password
                    // user can reset password later, because accounts have EmailConfirmed set to true
                    var result = !string.IsNullOrEmpty(user.Password)
                        ? await userManager.CreateAsync(identityUser, user.Password)
                        : await userManager.CreateAsync(identityUser);

                    if (result.Succeeded)
                    {
                        foreach (var claim in user.Claims)
                        {
                            await userManager.AddClaimAsync(identityUser, new System.Security.Claims.Claim(claim.Type, claim.Value));
                        }

                        foreach (var role in user.Roles)
                        {
                            await userManager.AddToRoleAsync(identityUser, role);
                        }
                    }
                }
            }
        }
    }
}
