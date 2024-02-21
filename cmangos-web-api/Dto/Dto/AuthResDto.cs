using System.Runtime.Serialization;

namespace Data.Dto
{
    public class AuthResDto
    {
        public uint UserId { get; set; }
        public string JwtToken { get; set; } = string.Empty;
        public double ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
    }
}
