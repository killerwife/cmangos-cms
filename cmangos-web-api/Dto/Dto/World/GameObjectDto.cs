namespace Data.Dto.World
{
    public class GameObjectDto
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public uint Guid { get; set; }
        public int? SpawnGroupId { get; set; }
        public bool HasDuplicate { get; set; }
    }
}
