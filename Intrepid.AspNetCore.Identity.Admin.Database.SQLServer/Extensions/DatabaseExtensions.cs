using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Intrepid.AspNetCore.Identity.Admin.Database.SQLServer.Extensions
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Register DbContexts for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        /// Configure the connection strings in AppSettings.json
        /// </summary>
        /// <typeparam name="TConfigurationDbContext"></typeparam>
        /// <typeparam name="TPersistedGrantDbContext"></typeparam>
        /// <typeparam name="TLogDbContext"></typeparam>
        /// <typeparam name="TIdentityDbContext"></typeparam>
        /// <typeparam name="TAuditLoggingDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="identityConnectionString"></param>
        /// <param name="configurationConnectionString"></param>
        /// <param name="persistedGrantConnectionString"></param>
        /// <param name="errorLoggingConnectionString"></param>
        /// <param name="auditLoggingConnectionString"></param>
        public static void RegisterSqlServerDbContexts<TIdentityDbContext>(this IServiceCollection services, string identityConnectionString)
             where TIdentityDbContext : IdentityDbContext

        {
            var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

            // Config DB for identity
            services.AddDbContextPool<TIdentityDbContext>(options => options.UseSqlServer(identityConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
        }

        ///// <summary>
        ///// Register DbContexts for IdentityServer ConfigurationStore and PersistedGrants and Identity
        ///// Configure the connection strings in AppSettings.json
        ///// </summary>
        ///// <typeparam name="TConfigurationDbContext"></typeparam>
        ///// <typeparam name="TPersistedGrantDbContext"></typeparam>
        ///// <typeparam name="TIdentityDbContext"></typeparam>
        ///// <param name="services"></param>
        ///// <param name="identityConnectionString"></param>
        ///// <param name="configurationConnectionString"></param>
        ///// <param name="persistedGrantConnectionString"></param>
        //public static void RegisterSqlServerDbContexts<TIdentityDbContext, TConfigurationDbContext,
        //    TPersistedGrantDbContext>(this IServiceCollection services,
        //    string identityConnectionString, string configurationConnectionString,
        //    string persistedGrantConnectionString)
        //    where TIdentityDbContext : DbContext
        //    where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
        //    where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
        //{
        //    var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

        //    // Config DB for identity
        //    services.AddDbContext<TIdentityDbContext>(options => options.UseSqlServer(identityConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

        //    // Config DB from existing connection
        //    services.AddConfigurationDbContext<TConfigurationDbContext>(options => options.ConfigureDbContext = b => b.UseSqlServer(configurationConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

        //    // Operational DB from existing connection
        //    services.AddOperationalDbContext<TPersistedGrantDbContext>(options => options.ConfigureDbContext = b => b.UseSqlServer(persistedGrantConnectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
        //}
    }
}
