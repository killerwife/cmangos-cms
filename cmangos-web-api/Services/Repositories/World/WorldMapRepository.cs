using Data.Model.DBC;

namespace Services.Repositories.World
{
    public class WorldMapRepository : IWorldMapRepository
    {
        private DBCRepository _dbcRepository;

        public WorldMapRepository(DBCRepository dbcRepository)
        {
            _dbcRepository = dbcRepository;
        }

        public WorldMapArea? GetWorldMapArea(int mapId, int zoneId)
        {
            KeyValuePair<int, WorldMapArea> areaEntry;
            if (zoneId == -1)
            {
                int worldMapId = 0;
                switch (mapId)
                {
                    case 0: worldMapId = 14; break;
                    case 1: worldMapId = 13; break;
                }
                areaEntry = _dbcRepository.WorldMapArea.Where(p => p.Key == worldMapId && mapId == p.Value.Map).SingleOrDefault();

                if (areaEntry.Equals(default(KeyValuePair<int, WorldMapArea>)))
                    return null;

                return areaEntry.Value;
            }
            else
            {
                switch (zoneId)
                {
                    case 3790: // Auchenai Crypts
                        return new WorldMapArea
                        {
                            Top = 285f,
                            Bottom = -300f,
                            Left = -665f,
                            Right = 254f,
                        };
                    default:
                        areaEntry = _dbcRepository.WorldMapArea.Where(p => p.Value.Area == zoneId && mapId == p.Value.Map).SingleOrDefault();
                        break;
                }

                if (areaEntry.Equals(default(KeyValuePair<int, WorldMapArea>)))
                    return null;

                return areaEntry.Value;
            }
        }
    }
}
