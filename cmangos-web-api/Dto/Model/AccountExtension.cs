namespace Data.Model
{
    public class AccountExtension
    {
        public uint Id { get; set; }
        public DateTime? Created { get; set; }
        public string? PendingEmail { get; set; } = string.Empty;
        public DateTime? EmailChanged { get; set; }
        public string? PendingToken { get; set; } = string.Empty;
        public DateTime? TokenChanged { get; set; }
    }
}
