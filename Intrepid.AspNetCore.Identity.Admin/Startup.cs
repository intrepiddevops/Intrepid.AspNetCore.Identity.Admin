using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Intrepid.AspNetCore.Identity.Admin.Database.SQLServer.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Intrepid.AspNetCore.Identity.Admin.Common.Settings;
using Intrepid.AspNetCore.Identity.Admin.Configuration;
using Newtonsoft.Json;
using System.IO;
using Intrepid.AspNetCore.Identity.Admin.BusinessLogic;
using Intrepid.AspNetCore.Identity.Admin.BusinessLogic.Mappers;
using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.Configurations.Mapper;
using System.Reflection;

namespace Intrepid.AspNetCore.Identity.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.RegisterSqlServerDbContexts<IdentityDbContext>(Configuration.GetConnectionString("DefaultConnection"));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>();
            //before continue ensure data is seeded
            //testing
            var  identityConfig = JsonConvert.DeserializeObject<IdentityDataConfiguration>(File.ReadAllText($@"{Environment.CurrentDirectory}\data\IdentityConfiguration.json"));
            services.AddSingleton(identityConfig);
            services.AddControllersWithViews();
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();
            this.RegisterPolicy(services);
            var assemlbies = new List<Assembly>();
            assemlbies.Add(typeof(IdentityUserProfile).Assembly);
            assemlbies.Add(typeof(IdentityRoleDTOProfile).Assembly);
            services.AddAutoMapper(assemlbies);
            services.AddScoped<RoleService>();
            services.AddScoped<IdentityService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
        
        private void RegisterPolicy(IServiceCollection services)
        {
            var policy = new Policy();
            this.Configuration.Bind("AdminPolicy", policy);
            var policies=policy.Roles.Select(x => x.Role).ToList();
            services.AddAuthorization(options =>
                options.AddPolicy("AdminManagerRole", policy =>
                    policy.RequireRole(policies))
            ); 
        }
    }
}
