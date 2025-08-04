using Data.Model.DBC;

namespace Services.Repositories.World
{
    public interface IWorldMapRepository
    {
        public WorldMapArea? GetWorldMapArea(int mapId, int zoneId);
    }
}
