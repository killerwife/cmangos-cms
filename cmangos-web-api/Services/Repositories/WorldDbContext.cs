using Data.Model;
using Data.Model.World;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services.Repositories
{
    public class WorldDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public WorldDbContext(IConfiguration configuration)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // in memory database used for simplicity, change to a real db for production applications
            //options.UseInMemoryDatabase("TestDb");
            var connectionString = Configuration.GetConnectionString("World");
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 35));
            options.UseMySql(connectionString, serverVersion)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<GameObject>(x => x.ToTable("gameobject"));
        }

        public DbSet<GameObject> GameObjects { get; set; }
    }
}
