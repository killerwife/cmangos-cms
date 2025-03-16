using Data.Dto.World;
using Data.Model.World;

namespace Services.Repositories.World
{
    public interface IWorldRepository
    {
        Task<CreatureWithMovementDto?> GetCreatureWithMovement(int zoneId, int guid);
        Task<(List<GameObjectWithSpawnGroup>, float, float, float, float)?> GetGameObjectsForZoneAndEntry(int mapId, int zoneId, uint entry);
        Task<string?> GetGameObjectEntryName(uint entry);
        Task<string?> GetCreatureEntryName(uint entry);
        Task<(List<CreatureWithSpawnGroup>, float, float, float, float)?> GetCreaturesForZoneAndEntry(int mapId, int zoneId, uint entry);
        Task<List<CreaturePredictData>> GetCreaturePredictions(string partial);
        Task<List<CreaturePredictData>> GetGameObjectPredictions(string partial);
        Task<List<uint>> GetGameObjectZones(uint entry);
        Task<List<uint>> GetCreatureZones(uint entry);
    }
}
