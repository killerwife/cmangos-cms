using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Services.Repositories;

namespace Services.Design
{
    public class DesignTimeMysql : IDesignTimeDbContextFactory<CmsDbContext>
    {
        CmsDbContext IDesignTimeDbContextFactory<CmsDbContext>.CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CmsDbContext>();
            string connectionString = "server=localhost; database=tbccms; user=mangos; password=mangos";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new CmsDbContext(optionsBuilder.Options);
        }
    }
}
