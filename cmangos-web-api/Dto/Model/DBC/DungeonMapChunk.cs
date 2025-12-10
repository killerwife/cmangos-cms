using DBFileReaderLib.Attributes;

namespace Data.Model.DBC
{
    public class DungeonMapChunk
    {
        [Index(true)]
        public uint ID;
        public uint Map;
        public uint WmoGroupId;
        public uint DungeonMap;
        public int Unk;
    }
}
