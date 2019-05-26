using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Moron.Server.Contexts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Moron.Server.DesignTimeDbFactories
{
    public class DesignTimePlayerDbContextFactory : IDesignTimeDbContextFactory<CommonContext>
    {
        public CommonContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
              //.SetBasePath(Directory.GetCurrentDirectory())
              //.AddJsonFile("appsettings.json")
              .Build();
            var builder = new DbContextOptionsBuilder<CommonContext>();
            //var connectionString = configuration.GetConnectionString("DefaultConnection");
            var connectionString = "Data Source=TegGamesDb.sqlite;";
            builder.UseSqlite(connectionString);
            return new CommonContext(builder.Options);
        }
    }
}
