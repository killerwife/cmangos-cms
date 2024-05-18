namespace Data.Dto.World
{
    public class CreatureMovementDto
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public class CreatureWithMovementDto
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public uint Guid { get; set; }
        public uint Map { get; set; }
        public List<CreatureMovementDto> Movement { get; set; } = new();

        public float Left { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
        public float Top { get; set; }
    }
}
