using Data.Model.World;

namespace Services.Repositories.World
{
    public interface IWorldRepository
    {
        Task<(List<GameObject>, float, float, float, float)?> GetGameObjectsForZoneAndEntry(int mapId, int zoneId, uint entry);
    }
}
