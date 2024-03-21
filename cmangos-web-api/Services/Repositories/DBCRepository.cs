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

        public Storage<WorldMapArea> AreaTable { get; set; }

        public void Load()
        {
            AreaTable = new Storage<WorldMapArea>(_dbcOptions.CurrentValue.DirectoryPath + "WorldMapArea.dbc");
        }
    }
}
