using Data.Dto.World;
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
            decimal minZoneX = (decimal)areaEntry.Value.Bottom, minZoneY = (decimal)areaEntry.Value.Left;
            decimal maxZoneX = (decimal)areaEntry.Value.Top, maxZoneY = (decimal)areaEntry.Value.Right;
            var gosWithSpawnGroup  = await _context.Database.SqlQuery<GameObjectWithSpawnGroup>(
                    $"SELECT gameobject.*, spawn_group.Id AS spawn_group_id FROM gameobject LEFT JOIN spawn_group_spawn ON gameobject.guid=spawn_group_spawn.Guid LEFT JOIN spawn_group ON spawn_group.Id=spawn_group_spawn.Id LEFT JOIN spawn_group_entry ON spawn_group_entry.Id=spawn_group.Id WHERE (gameobject.id={entry} OR spawn_group_entry.entry={entry}) AND gameobject.map={mapId} AND (spawn_group.type IS null OR spawn_group.type=1) AND position_x > {minZoneX} && position_x < {maxZoneX} && position_y > {minZoneY} && position_y < {maxZoneY}").ToListAsync();
            return (gosWithSpawnGroup, (float)minZoneY, (float)maxZoneY, (float)minZoneX, (float)maxZoneX);
        }

        public async Task<CreatureWithMovementDto?> GetCreatureWithMovement(int zoneId, int guid)
        {
            var areaEntry = _dbcRepository.AreaTable.Where(p => p.Value.Area == zoneId).SingleOrDefault();
            if (areaEntry.Equals(default(KeyValuePair<int, WorldMapArea>)))
                return null;

            var creature = _context.Creatures.Where(p => p.guid == guid).SingleOrDefault();
            if (creature == null)
                return null;

            var movement = new List<CreatureMovementDto>();
            var creatureMovement = _context.CreatureMovements.Where(p => p.Id == guid).ToList();
            if (creatureMovement == null || creatureMovement.Count == 0) // TODO: Add loading of path from template
            {
                var templateMovement = _context.CreatureMovementTemplates.Where(p => p.Entry == creature.id && p.PathId == 0).ToList();
                if (templateMovement == null || templateMovement.Count == 0)
                {
                    var spawn = _context.SpawnGroupSpawns.Where(p => p.Guid == guid).SingleOrDefault();
                    if (spawn != null)
                    {
                        var formation = _context.SpawnGroupFormations.Where(p => p.Id == spawn.Id).SingleOrDefault();
                        if (formation != null)
                        {
                            var waypoints = _context.WaypointPaths.Where(p => p.PathId == formation.PathId).ToList();
                            foreach (var waypoint in waypoints)
                                movement.Add(new CreatureMovementDto
                                {
                                    X = waypoint.PositionX,
                                    Y = waypoint.PositionY,
                                    Z = waypoint.PositionZ,
                                });
                        }
                    }
                }
                else
                {
                    foreach (var waypoint in templateMovement)
                        movement.Add(new CreatureMovementDto
                        {
                            X = waypoint.PositionX,
                            Y = waypoint.PositionY,
                            Z = waypoint.PositionZ,
                        });
                }
            }
            else
            {
                foreach (var waypoint in creatureMovement)
                    movement.Add(new CreatureMovementDto
                    {
                        X = waypoint.PositionX,
                        Y = waypoint.PositionY,
                        Z = waypoint.PositionZ,
                    });
            }

            return new CreatureWithMovementDto
            {
                Guid = creature.guid,
                X = (float)creature.position_x,
                Y = (float)creature.position_y,
                Z = (float)creature.position_z,
                Map = creature.map,
                Movement = movement,

                Bottom = areaEntry.Value.Bottom,
                Top = areaEntry.Value.Top,
                Right = areaEntry.Value.Right,
                Left = areaEntry.Value.Left
            };
        }

        public async Task<string?> GetEntryName(uint entry)
        {
            return (await _context.GameObjectTemplates.Where(p => p.Entry == entry).SingleOrDefaultAsync())?.Name;
        }
    }
}
