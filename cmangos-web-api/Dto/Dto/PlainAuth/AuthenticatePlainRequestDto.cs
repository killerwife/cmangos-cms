using System.ComponentModel.DataAnnotations;

namespace Data.Dto.PlainAuth
{
    public class AuthenticatePlainRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public string? Pin { get; set; }
    }
}
