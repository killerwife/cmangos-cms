using Data.Model.Db2;
using Data.Model.DBC;

namespace Services.Repositories.World
{
    public interface IWorldMapRepository
    {
        public UiMapAssignment? GetWorldMapArea(int mapId, int zoneId, int index);
        int PickIndexForXyz(float x, float y, float z, int zoneId);
        int PickIndexForWmoGroupId(int wmoGroupId, int zoneId);
    }
}
