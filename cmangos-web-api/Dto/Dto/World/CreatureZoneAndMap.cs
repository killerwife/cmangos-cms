using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Dto.World
{
    public class CreatureZoneAndMap
    {
        [Column("zoneId")]
        public uint ZoneId { get; set; }
        [Column("map")]
        public uint MapId { get; set; }
        [Column("wmoGroupId")]
        public int WmoGroupId { get; set; }
    }
}
