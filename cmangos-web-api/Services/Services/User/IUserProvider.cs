namespace cmangos_web_api.Auth
{
    public interface IUserProvider
    {
        UserInfo? CurrentUser { get; set; }
    }
}
