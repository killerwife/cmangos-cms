using Data.Model.DBC;
using DBFileReaderLib;

namespace Services.Repositories
{
    public class DBCRepository
    {
        public Storage<WorldMapArea> AreaTable { get; set; }

        public void Load()
        {
            AreaTable = new Storage<WorldMapArea>(@"C:\maps\wotlk\dbc\WorldMapArea.dbc");
        }
    }
}
