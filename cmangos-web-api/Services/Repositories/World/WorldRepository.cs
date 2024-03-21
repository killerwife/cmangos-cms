using Data.Enum;
using Data.Model.DBC;
using Data.Model.World;
using Microsoft.EntityFrameworkCore;

namespace Services.Repositories.World
{
    public class WorldRepository : IWorldRepository
    {
        private WorldDbContext _context;
        private DBCRepository _dbcRepository;

        public WorldRepository(WorldDbContext context, DBCRepository dbcRepository)
        {
            _context = context;
            _dbcRepository = dbcRepository;
        }

        public async Task<(List<GameObject>, float, float, float, float)?> GetGameObjectsForZoneAndEntry(int mapId, int zoneId, uint entry)
        {
            var areaEntry = _dbcRepository.AreaTable.Where(p => p.Value.Area == zoneId).SingleOrDefault();
            if (areaEntry.Equals(default(KeyValuePair<int, WorldMapArea>)))
                return null;
            decimal minZoneX = (decimal)areaEntry.Value.Bottom, minZoneY = (decimal)areaEntry.Value.Right;
            decimal maxZoneX = (decimal)areaEntry.Value.Top, maxZoneY = (decimal)areaEntry.Value.Left;
            var query = _context.GameObjects.Where(p => p.id == entry && p.map == mapId && p.position_x > minZoneX && p.position_x < maxZoneX && p.position_y > minZoneY && p.position_y < maxZoneY);
            Console.WriteLine(query.ToQueryString());
            var gameobjects = await query.ToListAsync();
            {
                var spawnGroupGos = await _context.Database.SqlQuery<GameObject>(
                    $"SELECT gameobject.* FROM gameobject LEFT JOIN spawn_group_spawn ON gameobject.guid=spawn_group_spawn.Guid LEFT JOIN spawn_group ON spawn_group.Id=spawn_group_spawn.Id LEFT JOIN spawn_group_entry ON spawn_group_entry.Id=spawn_group.Id WHERE spawn_group_entry.entry={entry} AND gameobject.map={mapId} AND spawn_group.type=1 AND gameobject.Id=0").ToListAsync();
                gameobjects.AddRange(spawnGroupGos);
            }
            return (gameobjects, (float)minZoneY, (float)maxZoneY, (float)minZoneX, (float)maxZoneX);
        }
    }
}
