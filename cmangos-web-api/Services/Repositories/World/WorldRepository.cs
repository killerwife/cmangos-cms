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

        public async Task<(List<GameObjectWithSpawnGroup>, float, float, float, float)?> GetGameObjectsForZoneAndEntry(int mapId, int zoneId, uint entry)
        {
            var areaEntry = _dbcRepository.AreaTable.Where(p => p.Value.Area == zoneId).SingleOrDefault();
            if (areaEntry.Equals(default(KeyValuePair<int, WorldMapArea>)))
                return null;
            decimal minZoneX = (decimal)areaEntry.Value.Bottom, minZoneY = (decimal)areaEntry.Value.Right;
            decimal maxZoneX = (decimal)areaEntry.Value.Top, maxZoneY = (decimal)areaEntry.Value.Left;
            var gosWithSpawnGroup  = await _context.Database.SqlQuery<GameObjectWithSpawnGroup>(
                    $"SELECT gameobject.*, spawn_group.Id AS spawn_group_id FROM gameobject LEFT JOIN spawn_group_spawn ON gameobject.guid=spawn_group_spawn.Guid LEFT JOIN spawn_group ON spawn_group.Id=spawn_group_spawn.Id LEFT JOIN spawn_group_entry ON spawn_group_entry.Id=spawn_group.Id WHERE (gameobject.id={entry} OR spawn_group_entry.entry={entry}) AND gameobject.map={mapId} AND (spawn_group.type IS null OR spawn_group.type=1) AND position_x > {minZoneX} && position_x < {maxZoneX} && position_y > {minZoneY} && position_y < {maxZoneY}").ToListAsync();
            return (gosWithSpawnGroup, (float)minZoneY, (float)maxZoneY, (float)minZoneX, (float)maxZoneX);
        }
    }
}
