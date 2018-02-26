using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace CoralTime.DAL
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        //public AppDbContext Create(DbContextFactoryOptions options)
        //{
        //    // Used only for EF .NET Core CLI tools (update database/migrations etc.)
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(basePath: Directory.GetCurrentDirectory()) // AppContext.BaseDirectory
        //        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        //    var config = builder.Build();

        //    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        //    optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

        //    return new AppDbContext(optionsBuilder.Options);
        //}

        public AppDbContext CreateDbContext(string[] args)
        {
            // Used only for EF .NET Core CLI tools (update database/migrations etc.)
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath: Directory.GetCurrentDirectory()) // AppContext.BaseDirectory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
