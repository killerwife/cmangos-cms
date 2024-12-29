namespace Data.Dto.World
{
    public class CreaturePredictResponseDto
    {
        public string Name { get; set; } = string.Empty;
        public uint Map { get; set; }
        public uint Zone { get; set; }
        public uint Entry { get; set; }
    }
}
