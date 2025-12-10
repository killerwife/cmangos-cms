using Configs;
using Data.Model.DBC;
using DBFileReaderLib;
using Microsoft.Extensions.Options;

namespace Services.Repositories
{
    public class DBCRepository
    {
        private IOptionsMonitor<DbcConfig> _dbcOptions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DBCRepository(IOptionsMonitor<DbcConfig> dbcOptions)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _dbcOptions = dbcOptions;
        }

        public Storage<WorldMapArea> WorldMapArea { get; set; }
        public Storage<AreaTable> Areas { get; set; }
        public Storage<WmoAreaTable> WmoAreas { get; set; }
        public Storage<DungeonMapChunk> DungeonMapChunks { get; set; }
        public Storage<DungeonMap> DungeonMaps { get; set; }

        public void Load()
        {
            WorldMapArea = new Storage<WorldMapArea>(_dbcOptions.CurrentValue.DirectoryPath + "WorldMapArea.dbc");
            Areas = new Storage<AreaTable>(_dbcOptions.CurrentValue.DirectoryPath + "AreaTable.dbc");
            WmoAreas = new Storage<WmoAreaTable>(_dbcOptions.CurrentValue.DirectoryPath + "WMOAreaTable.dbc");
            DungeonMapChunks = new Storage<DungeonMapChunk>(_dbcOptions.CurrentValue.DirectoryPath + "DungeonMapChunk.dbc");
            DungeonMaps = new Storage<DungeonMap>(_dbcOptions.CurrentValue.DirectoryPath + "DungeonMap.dbc");
        }
    }
}
