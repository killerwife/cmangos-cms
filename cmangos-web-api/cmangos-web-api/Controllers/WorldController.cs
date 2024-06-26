﻿using Data.Dto.User;
using Data.Dto.World;
using Data.Model.World;
using Microsoft.AspNetCore.Mvc;
using Services.Repositories.World;
using System.ComponentModel.DataAnnotations;

namespace cmangos_web_api.Controllers
{
    [Route("world")]
    public class WorldController : ControllerBase
    {
        private IWorldRepository _worldRepository;

        public WorldController(IWorldRepository worldRepository)
        {
            _worldRepository = worldRepository;
        }

        /// <summary>
        /// Returns gameobject xyz for zone with given entry
        /// </summary>
        /// <response code="200">Gameobject information</response>
        [HttpGet("gameobject/{mapId}/{zone}/{entry}")]
        public async Task<ActionResult<GameObjectListDto>> GetUserInfo([Required][FromRoute] int mapId, [Required][FromRoute] int zone, [Required][FromRoute] uint entry)
        {
            var data = await _worldRepository.GetGameObjectsForZoneAndEntry(mapId, zone, entry);
            if (data == null)
                return BadRequest();
            var result = new GameObjectListDto();
            result.Count = data.Value.Item1.Count;
            result.Left = data.Value.Item2;
            result.Right = data.Value.Item3;
            result.Bottom = data.Value.Item4;
            result.Top = data.Value.Item5;
            foreach (var gameObject in data.Value.Item1)
            {
                result.Items.Add(new GameObjectDto
                {
                    X = (float)gameObject.position_x,
                    Y = (float)gameObject.position_y,
                    Z = (float)gameObject.position_z,
                    Guid = gameObject.guid,
                    SpawnGroupId = gameObject.spawn_group_id
                });
            }
            return Ok(result);
        }

        /// <summary>
        /// Returns creature xyz for zone with given entry and its movement
        /// </summary>
        /// <response code="200">Gameobject information</response>
        [HttpGet("creature/{zone}/{guid}")]
        public async Task<ActionResult<CreatureWithMovementDto>> GetCreatureMovement([Required][FromRoute] int guid, [Required][FromRoute] int zone)
        {
            var result = await _worldRepository.GetCreatureWithMovement(zone, guid);
            return result != null ? Ok(result) : BadRequest();
        }
    }
}
