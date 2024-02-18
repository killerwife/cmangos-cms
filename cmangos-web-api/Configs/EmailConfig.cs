namespace Data.Config;

public class EmailConfig
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public bool RequireSsl { get; set; }
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string SenderEmail { get; set; } = "";
    public string SenderAlias { get; set; } = "";
}