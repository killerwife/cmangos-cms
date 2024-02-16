using System.Runtime.Serialization;

namespace Data.Dto
{
    public class AuthResDto
    {
        [DataMember(Name = "client_id")]
        public string Key { get; set; } = string.Empty;
        [DataMember(Name = "access_token")]
        public string JwtToken { get; set; } = string.Empty;
        [DataMember(Name = "expires_in")]
        public double ExpiresIn { get; set; }
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
    }
}
