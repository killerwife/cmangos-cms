
namespace Services.Repositories.Cms
{
    public class EntityExt : IEntityExt
    {
        private CmsDbContext _dbContext;

        public EntityExt(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<uint> GetGameObjectZones(uint entry)
        {
            return _dbContext.GameObjectZones.Where(p => p.Entry == entry).Select(q => q.ZoneId).ToList();
        }
    }
}
