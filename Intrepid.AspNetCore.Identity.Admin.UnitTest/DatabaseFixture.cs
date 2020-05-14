using AutoMapper;
using Intrepid.AspNetCore.Identity.Admin.BusinessLogic.Mappers;
using Intrepid.AspNetCore.Identity.Admin.Database.SQLServer.Extensions;
using MartinCostello.SqlLocalDb;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using Xunit;

namespace Intrepid.AspNetCore.Identity.Admin.UnitTest
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
    public class DatabaseFixture : IDisposable
    {
        private string AdminDB { get { return "AdminDB"; } }
        private string AdminDBInstance { get { return "AdminDB"; } }
        public ServiceCollection ServiceCollection { get; set; }
        private string Path => System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public IServiceProvider Provider { get; private set; }
        public DatabaseFixture()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                           .SetBasePath(Directory.GetCurrentDirectory())
                                           .AddJsonFile(@"appsettings.json");

            IConfigurationRoot config = builder.Build();
            this.ServiceCollection = new ServiceCollection();
            CreateOrStartLocalDBInstances();
            AddServiceCollection(config);
            Provider = this.ServiceCollection.BuildServiceProvider();
            //update the provider migration
            var dbcontext = Provider.GetService<IdentityDbContext>();
            dbcontext.Database.Migrate();
        }

        private void AddServiceCollection(IConfigurationRoot config)
        {
            //add dummy log
            ServiceCollection.AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(config);
            });
            //add the mapper
            ServiceCollection.AddAutoMapper(typeof(IdentityUserProfile).Assembly);
            ServiceCollection.RegisterSqlServerDbContexts<IdentityDbContext>(config.GetConnectionString("DefaultConnection"));
        }

        private void CreateOrStartLocalDBInstances()
        {
            // Check that SQL Server LocalDB is installed

            var localDB = new SqlLocalDbApi();

            if (!localDB.IsLocalDBInstalled())
            {
                throw new NotSupportedException("SQL LocalDB is not installed.");
            }

            // Get the configured SQL LocalDB instance to store the TODO items in, creating it if it does not exist
            //IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();
            ISqlLocalDbInstanceInfo adminInstance = localDB.GetOrCreateInstance($"{AdminDBInstance}");

            // Ensure that the SQL LocalDB instance is running and start it if not already running
            if (!adminInstance.IsRunning)
            {
                adminInstance.Manage().Start();
            }
            using var connection=adminInstance.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"Create DATABASE {AdminDB}";
            command.ExecuteNonQuery();
            connection.Close();
        }
        public void Dispose()
        {
            var localDB = new SqlLocalDbApi();
            ISqlLocalDbInstanceInfo adminInstance = localDB.GetOrCreateInstance($"{AdminDBInstance}");
            using var connection = adminInstance.CreateConnection();
            connection.Open();
            using var commandSingle = connection.CreateCommand();
            commandSingle.CommandText = $"alter database {AdminDB} set single_user with rollback immediate";
            commandSingle.ExecuteNonQuery();
            using var command = connection.CreateCommand();
            command.CommandText = $"drop database {AdminDB}";
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
