using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services.Repositories
{
    public class CmsDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public CmsDbContext(IConfiguration configuration)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // in memory database used for simplicity, change to a real db for production applications
            //options.UseInMemoryDatabase("TestDb");
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 35));
            var connectionString = Configuration.GetConnectionString("Cms");
            options.UseMySql(connectionString, serverVersion)
                .LogTo(Console.WriteLine, LogLevel.Debug)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AccountExtension>(x => x.ToTable("account_ext"));
            builder.Entity<RefreshToken>(x => x.ToTable("refresh_token"));
        }


        public DbSet<AccountExtension> AccountsExt { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
    }
}
