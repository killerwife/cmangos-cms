using System.ComponentModel.DataAnnotations;

namespace Data.Model
{
    public class Account
    {
        [Key]
        public uint id { get; set; }
        public string username { get; set; } = string.Empty;
        public string? v { get; set; } = string.Empty;
        public string? s { get; set; } = string.Empty;
        public string lockedIp { get; set; } = string.Empty;
        public string? email { get; set; } = string.Empty;
        public string? token { get; set; } = string.Empty;
        public int locked { get; set; }
    }
}
