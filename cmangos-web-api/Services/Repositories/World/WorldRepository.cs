using Common;
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
        private IWorldMapRepository _worldMapRepository;

        public WorldRepository(WorldDbContext context, IWorldMapRepository worldMapRepository)
        {
            _context = context;
            _worldMapRepository = worldMapRepository;
        }

        public async Task<(List<GameObjectWithSpawnGroup>, float, float, float, float)?> GetGameObjectsForZoneAndEntry(int mapId, int zoneId, uint entry)
        {
            WorldMapArea? worldMapArea = _worldMapRepository.GetWorldMapArea(mapId, zoneId);
            if (worldMapArea == null)
                return null;

            float minZoneX = worldMapArea.Bottom, minZoneY = worldMapArea.Left;
            float maxZoneX = worldMapArea.Top, maxZoneY = worldMapArea.Right;
            var gosWithSpawnGroup  = await _context.Database.SqlQuery<GameObjectWithSpawnGroup>(
                    $"SELECT gameobject.*, spawn_group.Id AS spawn_group_id FROM gameobject LEFT JOIN spawn_group_spawn ON gameobject.guid=spawn_group_spawn.Guid LEFT JOIN spawn_group ON spawn_group.Id=spawn_group_spawn.Id LEFT JOIN spawn_group_entry ON spawn_group_entry.Id=spawn_group.Id LEFT JOIN gameobject_spawn_entry ON gameobject.guid=gameobject_spawn_entry.guid WHERE (gameobject.id={entry} OR spawn_group_entry.entry={entry} OR gameobject_spawn_entry.entry={entry}) AND gameobject.map={mapId} AND (spawn_group.type IS null OR spawn_group.type=1) AND position_x > {minZoneX} && position_x < {maxZoneX} && position_y > {minZoneY} && position_y < {maxZoneY} ORDER BY gameobject.guid").ToListAsync();
            return (gosWithSpawnGroup, (float)minZoneY, (float)maxZoneY, (float)minZoneX, (float)maxZoneX);
        }

        public async Task<(List<CreatureWithSpawnGroup>, float, float, float, float)?> GetCreaturesForZoneAndEntry(int mapId, int zoneId, uint entry)
        {
            WorldMapArea? worldMapArea = _worldMapRepository.GetWorldMapArea(mapId, zoneId);
            if (worldMapArea == null)
                return null;

            decimal minZoneX = (decimal)worldMapArea.Bottom, minZoneY = (decimal)worldMapArea.Left;
            decimal maxZoneX = (decimal)worldMapArea.Top, maxZoneY = (decimal)worldMapArea.Right;
            var gosWithSpawnGroup = await _context.Database.SqlQuery<CreatureWithSpawnGroup>(
                    $"SELECT creature.*, spawn_group.Id AS spawn_group_id FROM creature LEFT JOIN spawn_group_spawn ON creature.guid=spawn_group_spawn.Guid LEFT JOIN spawn_group ON spawn_group.Id=spawn_group_spawn.Id LEFT JOIN spawn_group_entry ON spawn_group_entry.Id=spawn_group.Id LEFT JOIN creature_spawn_entry ON creature.guid=creature_spawn_entry.guid WHERE (creature.id={entry} OR spawn_group_entry.entry={entry} OR creature_spawn_entry.entry={entry}) AND creature.map={mapId} AND (spawn_group.type IS null OR spawn_group.type=0) AND position_x > {minZoneX} && position_x < {maxZoneX} && position_y > {minZoneY} && position_y < {maxZoneY}").ToListAsync();
            return (gosWithSpawnGroup, (float)minZoneY, (float)maxZoneY, (float)minZoneX, (float)maxZoneX);
        }

        public async Task<CreatureWithMovementDto?> GetCreatureWithMovement(int mapId, int zoneId, int guid)
        {
            WorldMapArea? worldMapArea = _worldMapRepository.GetWorldMapArea(mapId, zoneId);
            if (worldMapArea == null)
                return null;

            var creature = _context.Creatures.Where(p => p.guid == guid).SingleOrDefault();
            if (creature == null)
                return null;

            var movement = new List<CreatureMovementDto>();
            var creatureMovement = _context.CreatureMovements.Where(p => p.Id == guid).ToList();
            if (creatureMovement == null || creatureMovement.Count == 0)
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

            var name = await GetCreatureEntryName(creature.id);

            return new CreatureWithMovementDto
            {
                Guid = creature.guid,
                X = (float)creature.position_x,
                Y = (float)creature.position_y,
                Z = (float)creature.position_z,
                Entry = creature.id,
                Name = name!,
                Map = creature.map,
                Movement = movement,

                Bottom = worldMapArea.Bottom,
                Top = worldMapArea.Top,
                Right = worldMapArea.Right,
                Left = worldMapArea.Left
            };
        }

        public async Task<string?> GetGameObjectEntryName(uint entry)
        {
            return (await _context.GameObjectTemplates.Where(p => p.Entry == entry).SingleOrDefaultAsync())?.Name;
        }

        public async Task<string?> GetCreatureEntryName(uint entry)
        {
            return (await _context.CreatureTemplates.Where(p => p.Entry == entry).SingleOrDefaultAsync())?.Name;
        }

        public async Task<List<CreaturePredictData>> GetGameObjectPredictions(string partial)
        {
            var mapIds = WorldConstants.SupportedMaps;
            var result = _context.GameObjectTemplates.Where(p => p.Name.Contains(partial)).Select(p => new CreaturePredictData
            {
                Entry = p.Entry,
                Name = p.Name,
                Map = _context.GameObjects.Where(q => q.id == p.Entry && mapIds.Contains(q.map)).FirstOrDefault()!.map,
                ZoneId = _context.GameObjects.Where(q => q.id == p.Entry && mapIds.Contains(q.map)).Join(_context.GameObjectZones, x => x.guid, y => y.Guid, (x, y) => y.ZoneId).FirstOrDefault()!,
            });
            return await result.ToListAsync();
        }

        public async Task<List<CreaturePredictData>> GetCreaturePredictions(string partial)
        {
            var mapIds = WorldConstants.SupportedMaps;
            var result = _context.CreatureTemplates.Where(p => p.Name.Contains(partial)).Select(p => new CreaturePredictData
            {
                Entry = p.Entry,
                Name = p.Name,
                Map = _context.Creatures.Where(q => q.id == p.Entry && mapIds.Contains(q.map)).FirstOrDefault()!.map,
                ZoneId = _context.Creatures.Where(q => q.id == p.Entry && mapIds.Contains(q.map)).Join(_context.CreatureZones, x => x.guid, y => y.Guid, (x, y) => y.ZoneId).FirstOrDefault()!,
            });
            return await result.ToListAsync();
        }

        public async Task<List<CreatureZoneAndMap>> GetGameObjectZones(uint entry)
        {
            return await _context.Database.SqlQuery<CreatureZoneAndMap>($"SELECT DISTINCT gameobject_zone.zoneId, gameobject.map FROM gameobject LEFT JOIN gameobject_zone ON gameobject_zone.guid=gameobject.guid LEFT JOIN spawn_group_spawn ON gameobject.guid=spawn_group_spawn.Guid LEFT JOIN spawn_group ON spawn_group.Id=spawn_group_spawn.Id LEFT JOIN spawn_group_entry ON spawn_group_entry.Id=spawn_group.Id LEFT JOIN gameobject_spawn_entry ON gameobject.guid=gameobject_spawn_entry.guid WHERE (gameobject.id={entry} OR spawn_group_entry.entry={entry} OR gameobject_spawn_entry.entry={entry}) AND (spawn_group.type IS null OR spawn_group.type=1) AND zoneId IS NOT NULL").ToListAsync();
        }

        public async Task<List<CreatureZoneAndMap>> GetCreatureZones(uint entry)
        {
            return await _context.Database.SqlQuery<CreatureZoneAndMap>($"SELECT DISTINCT creature_zone.zoneId, creature.map FROM creature LEFT JOIN creature_zone ON creature_zone.guid=creature.guid LEFT JOIN spawn_group_spawn ON creature.guid=spawn_group_spawn.Guid LEFT JOIN spawn_group ON spawn_group.Id=spawn_group_spawn.Id LEFT JOIN spawn_group_entry ON spawn_group_entry.Id=spawn_group.Id LEFT JOIN creature_spawn_entry ON creature.guid=creature_spawn_entry.guid WHERE (creature.id={entry} OR spawn_group_entry.entry={entry} OR creature_spawn_entry.entry={entry}) AND (spawn_group.type IS null OR spawn_group.type=0) AND zoneId IS NOT NULL").ToListAsync();
        }
    }
}
