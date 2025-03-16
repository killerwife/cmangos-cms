using Microsoft.EntityFrameworkCore;

namespace Data.Model
{
    [PrimaryKey(nameof(Entry), nameof(ZoneId))]
    public class GameObjectCmsZone
    {
        public uint Entry { get; set; }
        public uint ZoneId { get; set; }
    }
}
