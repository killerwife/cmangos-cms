using DBFileReaderLib.Attributes;

namespace Data.Model.DBC
{
    public sealed class DBFileAttribute : Attribute
    {
        public string FileName { get; }

        public DBFileAttribute(string fileName)
        {
            FileName = fileName;
        }
    }

    [DBFile("WorldMapArea")]
    public sealed class WorldMapArea
    {
        [Index(true)]
        public uint ID;
        public uint Map;
        public uint Area;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Name;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;
        public int VirtualMap;
        public int DungeonMap;
        public uint OtherMap;
    }
}
