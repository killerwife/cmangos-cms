namespace Data.Dto.User
{
    public class UserInfoDto
    {
        public string Email { get; set; } = string.Empty;
        public bool HasAuthenticator { get; set; }
    }
}
