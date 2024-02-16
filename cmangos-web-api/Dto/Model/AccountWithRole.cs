namespace Data.Model
{
    public class AccountWithRole
    {
        public string Uuid { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
