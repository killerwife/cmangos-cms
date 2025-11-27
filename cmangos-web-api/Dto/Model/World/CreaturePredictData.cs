namespace Data.Model.World
{
    public class CreaturePredictData
    {
        public string Name { get; set; } = string.Empty;
        public uint Entry { get; set; }
        public uint? Map { get; set; }
        public uint? ZoneId { get; set; }
        public float? X { get; set; }
        public float? Y { get; set; }
        public float? Z { get; set; }
        public int? WmoGroupId { get; set; }
    }
}
