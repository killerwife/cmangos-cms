namespace Data.Model
{
    public class AccountExtension
    {
        public uint Id { get; set; }
        public string? PendingEmailToken { get; set; }
        public string? PendingEmail { get; set; } = string.Empty;
        public DateTime? PendingEmailTokenSent { get; set; }
        public DateTime? EmailChanged { get; set; }
        public string? PendingToken { get; set; } = string.Empty;
        public DateTime? TokenChanged { get; set; }
        public DateTime? PasswordRecoverySent { get; set; }
        public string? PasswordRecoveryToken {  get; set; }
    }
}
