using DBFileReaderLib.Attributes;

namespace Data.Model.DBC
{
    public class DungeonMap
    {
        [Index(true)]
        public uint ID;
        public uint Map;
        public uint Index;
        public float Right;
        public float Left;
        public float Top;
        public float Bottom;
        public int Unk;
    }
}
