using Data.Dto.World;
using Data.Model.World;

namespace Services.Repositories.World
{
    public interface IWorldRepository
    {
        Task<CreatureWithMovementDto?> GetCreatureWithMovement(int zoneId, int guid);
        Task<(List<GameObjectWithSpawnGroup>, float, float, float, float)?> GetGameObjectsForZoneAndEntry(int mapId, int zoneId, uint entry);
    }
}
