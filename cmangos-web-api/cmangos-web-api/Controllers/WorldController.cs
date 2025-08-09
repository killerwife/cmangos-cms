using Common;
using Data.Dto.User;
using Data.Dto.World;
using Data.Enum;
using Data.Model.World;
using Microsoft.AspNetCore.Mvc;
using Services.Repositories;
using Services.Repositories.Cms;
using Services.Repositories.World;
using System.ComponentModel.DataAnnotations;

namespace cmangos_web_api.Controllers
{
    [Route("world")]
    public class WorldController : ControllerBase
    {
        private IWorldRepository _worldRepository;
        private IEntityExt _entityExt;
        private DBCRepository _dbcRepository;

        public WorldController(IWorldRepository worldRepository, IEntityExt entityExt, DBCRepository dbcRepository)
        {
            _worldRepository = worldRepository;
            _entityExt = entityExt;
            _dbcRepository = dbcRepository;
        }

        private static bool FloatComparison(decimal x, decimal y, decimal precision)
        {
            return Math.Abs(x - y) < precision;
        }

        /// <summary>
        /// Returns gameobject xyz for zone with given entry
        /// </summary>
        /// <response code="200">Gameobject information</response>
        [HttpGet("gameobjects/{mapId}/{zone}/{entry}")]
        public async Task<ActionResult<GameObjectListDto>> GetGameObjects([Required][FromRoute] int mapId, [Required][FromRoute] int zone, [Required][FromRoute] uint entry)
        {
            var data = await _worldRepository.GetGameObjectsForZoneAndEntry(mapId, zone, entry);
            if (data == null)
                return BadRequest();
            var precision = 0.2M;
            var result = new GameObjectListDto();
            result.Name = await _worldRepository.GetGameObjectEntryName(entry);
            {
                var zones = await _worldRepository.GetGameObjectZones(entry);
                foreach (var zoneItem in zones)
                {
                    if (zoneItem.ZoneId == 0)
                        continue;
                    var name = _dbcRepository.Areas.SingleOrDefault(p => p.Value.ID == zoneItem.ZoneId).Value.area_name;
                    result.Zones.Add(new EntityZone
                    {
                        MapId = zoneItem.MapId,
                        ZoneId = zoneItem.ZoneId,
                        Name = name
                    });
                }
            }
            if (result.Zones.Count == 0)
            {
                var mapIds = WorldConstants.SupportedMaps;
                var zones = _dbcRepository.WorldMapArea.Where(p => p.Value.Area != 0 && mapIds.Contains(p.Value.Map));
                foreach (var zoneData in zones)
                    result.Zones.Add(new EntityZone
                    {
                        MapId = zoneData.Value.Map,
                        ZoneId = zoneData.Value.Area,
                        Name = zoneData.Value.Name,
                    });
            }
            result.Count = data.Value.Item1.Count;
            result.Left = data.Value.Item2;
            result.Right = data.Value.Item3;
            result.Bottom = data.Value.Item4;
            result.Top = data.Value.Item5;
            foreach (var gameObject in data.Value.Item1)
            {
                var duplicates = data.Value.Item1.Where(p => p.guid != gameObject.guid && FloatComparison(p.position_x, gameObject.position_x, precision)
                        && FloatComparison(p.position_y, gameObject.position_y, precision) && FloatComparison(p.position_z, gameObject.position_z, precision)).Select(p => p.guid.ToString()).Distinct();

                if (result.Items.Any(p => p.Guid == gameObject.guid))
                    continue;
                
                result.Items.Add(new GameObjectDto
                {
                    X = (float)gameObject.position_x,
                    Y = (float)gameObject.position_y,
                    Z = (float)gameObject.position_z,
                    Guid = gameObject.guid,
                    SpawnGroupId = gameObject.spawn_group_id,
                    HasDuplicate = duplicates.Count() > 0,
                    Duplicates = string.Join(' ', duplicates)
                });
            }
            return Ok(result);
        }

        /// <summary>
        /// Returns creature xyz for zone with given entry and its movement
        /// </summary>
        /// <response code="200">Gameobject information</response>
        [HttpGet("creature/{mapId}/{zone}/{guid}")]
        public async Task<ActionResult<CreatureWithMovementDto>> GetCreatureMovement([Required][FromRoute] int guid, [Required][FromRoute] int zone, [Required][FromRoute] int mapId)
        {
            var result = await _worldRepository.GetCreatureWithMovement(mapId, zone, guid);
            return result != null ? Ok(result) : BadRequest();
        }

        /// <summary>
        /// Returns creature xyz for zone with given entry
        /// </summary>
        /// <response code="200">Gameobject information</response>
        [HttpGet("creatures/{mapId}/{zone}/{entry}")]
        public async Task<ActionResult<CreatureListDto>> GetCreatures([Required][FromRoute] int mapId, [Required][FromRoute] int zone, [Required][FromRoute] uint entry)
        {
            var data = await _worldRepository.GetCreaturesForZoneAndEntry(mapId, zone, entry);
            if (data == null)
                return BadRequest();
            var result = new CreatureListDto();
            var mapIds = WorldConstants.SupportedMaps;
            {
                var zones = await _worldRepository.GetCreatureZones(entry);
                foreach (var zoneItem in zones)
                {
                    if (zoneItem.ZoneId == 0)
                        continue;
                    var name = _dbcRepository.Areas.SingleOrDefault(p => p.Value.ID == zoneItem.ZoneId).Value.area_name;
                    result.Zones.Add(new EntityZone
                    {
                        MapId = zoneItem.MapId,
                        ZoneId = zoneItem.ZoneId,
                        Name = name
                    });
                }
            }
            if (result.Zones.Count() == 0)
            {
                var zones = _dbcRepository.WorldMapArea.Where(p => p.Value.Area != 0 && mapIds.Contains(p.Value.Map));
                foreach (var zoneData in zones)
                    result.Zones.Add(new EntityZone
                    {
                        MapId = zoneData.Value.Map,
                        ZoneId = zoneData.Value.Area,
                        Name = zoneData.Value.Name,
                    });
            }
            result.Name = await _worldRepository.GetCreatureEntryName(entry);
            result.Count = data.Value.Item1.Count;
            result.Left = data.Value.Item2;
            result.Right = data.Value.Item3;
            result.Bottom = data.Value.Item4;
            result.Top = data.Value.Item5;
            foreach (var creature in data.Value.Item1)
            {
                result.Items.Add(new CreatureDto
                {
                    X = (float)creature.position_x,
                    Y = (float)creature.position_y,
                    Z = (float)creature.position_z,
                    Guid = creature.guid,
                    SpawnGroupId = creature.spawn_group_id,
                });
            }
            return Ok(result);
        }

        /// <summary>
        /// Returns creature xyz for zone with given entry
        /// </summary>
        /// <response code="200">Gameobject information</response>
        [HttpPost("gameobjects/predict")]
        public async Task<ActionResult<List<CreaturePredictResponseDto>>> PostGameObjectsPredictions([FromBody] CreaturePredictRequestDto predictDto)
        {
            var results = await _worldRepository.GetGameObjectPredictions(predictDto.Name);
            var response = new List<CreaturePredictResponseDto>();
            foreach (var result in results)
            {
                if (result.Map == null)
                    continue;

                response.Add(new CreaturePredictResponseDto
                {
                    Entry = result.Entry,
                    Map = result.Map.Value,
                    Name = result.Name,
                    Zone = result.ZoneId ?? 0
                });
            }
            return Ok(response);
        }

        /// <summary>
        /// Returns creature xyz for zone with given entry
        /// </summary>
        /// <response code="200">Creature information</response>
        [HttpPost("creatures/predict")]
        public async Task<ActionResult<List<CreaturePredictResponseDto>>> PostCreaturePredictions([FromBody] CreaturePredictRequestDto predictDto)
        {
            var results = await _worldRepository.GetCreaturePredictions(predictDto.Name);
            var response = new List<CreaturePredictResponseDto>();
            foreach (var result in results)
            {
                if (result.Map == null)
                    continue;

                response.Add(new CreaturePredictResponseDto
                {
                    Entry = result.Entry,
                    Map = result.Map.Value,
                    Name = result.Name,
                    Zone = result.ZoneId ?? 0
                });
            }
            return Ok(response);
        }
    }
}
