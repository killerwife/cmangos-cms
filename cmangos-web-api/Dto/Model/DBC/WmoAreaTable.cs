using DBFileReaderLib.Attributes;

namespace Data.Model.DBC
{
    [DBFile("WMOAreaTable")]
    public class WmoAreaTable
    {
        [Index(true)]
        public uint ID;
        public uint ZoneId;
        public uint Unk;
        public uint WmoGroupId;
        public uint Unk2;
        public uint Unk3;
        public uint Unk4;
        public uint Unk5;
        public uint Unk6;
        public uint Unk7;
        public uint Unk8;
        public string WmoAreaOverride;
    }
}
