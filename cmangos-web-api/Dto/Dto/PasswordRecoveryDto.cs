namespace Data.Dto
{
    public class PasswordRecoveryDto
    {
        public string NewPassword { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
