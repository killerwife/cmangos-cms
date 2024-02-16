namespace cmangos_web_api.Configs
{
    public class AuthConfig
    {
        public string JwtPrivate { get; set; } = string.Empty;
        public string JwtPublic { get; set; } = string.Empty;
        public long JwtExpirationSeconds { get; set; }
        public long RefreshExpirationSeconds { get; set; }
    }
}
