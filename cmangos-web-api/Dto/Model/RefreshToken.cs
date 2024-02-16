using System.ComponentModel.DataAnnotations;

namespace Data.Model
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string UserUuid { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.Now >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; } = string.Empty;
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; } = string.Empty;
        public string ReplacedByToken { get; set; } = string.Empty;
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
