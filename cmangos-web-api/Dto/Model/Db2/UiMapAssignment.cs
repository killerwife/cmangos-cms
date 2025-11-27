using Data.Enum;
using Data.Model.DBC;
using System;

namespace Data.Model.Db2
{
    public class UiMapAssignment
    {
        public int MapId { get; set; }
        public int ZoneId { get; set; }
        public int Index { get; set; }
        public double MinX { get; set; } // left
        public double MaxX { get; set; } // right
        public double MinY { get; set; } // bottom
        public double MaxY { get; set; } // top
        public double MinZ { get; set; }
        public double MaxZ { get; set; }
        public List<int> WmoGroupId { get; set; }

        public UiMapAssignment(int mapId, int zoneId, int index, WorldMapArea mapArea)
        {
            MapId = mapId;
            ZoneId = zoneId;
            Index = index;
            MinX = mapArea.Left;
            MinY = mapArea.Bottom;
            MaxX = mapArea.Right;
            MaxY = mapArea.Top;
            MaxZ = 1000000;
            MinZ = -1000000;
            WmoGroupId = new List<int>();
        }

        public UiMapAssignment(int mapId, int zoneId, int index, double minX, double minY, double minZ, double maxX, double maxY, double maxZ, int wmoGroupId)
        {
            MapId = mapId;
            ZoneId = zoneId;
            Index = index;
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            MinZ = minZ;
            MaxZ = maxZ;
            WmoGroupId = new List<int>
            {
                wmoGroupId
            };
        }

        public UiMapAssignment(int mapId, int zoneId, int index, double minX, double minY, double minZ, double maxX, double maxY, double maxZ, List<int> wmoGroupId)
        {
            MapId = mapId;
            ZoneId = zoneId;
            Index = index;
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            MinZ = minZ;
            MaxZ = maxZ;
            WmoGroupId = wmoGroupId;
        }
    }
}
