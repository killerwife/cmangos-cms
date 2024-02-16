namespace Data.Dto.PlainAuth
{
    public class AuthenticatePlainRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
    }
}
