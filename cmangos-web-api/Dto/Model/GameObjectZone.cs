using Microsoft.EntityFrameworkCore;

namespace Data.Model
{
    [PrimaryKey(nameof(Entry), nameof(ZoneId))]
    public class GameObjectZone
    {
        public uint Entry { get; set; }
        public uint ZoneId { get; set; }
    }
}
