namespace Data.Dto.World
{
    public class CreatureListDto
    {
        public List<CreatureDto> Items { get; set; } = new List<CreatureDto>();
        public int Count { get; set; } = 0;
        public float Left { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
        public float Top { get; set; }
        public string? Name { get; set; }
        public List<EntityZone> Zones { get; set; } = new();
        public List<int> MapIndices { get; set; } = new();
    }
}
