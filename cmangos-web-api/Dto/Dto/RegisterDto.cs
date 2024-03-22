
using System.ComponentModel.DataAnnotations;

namespace Data.Dto
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public string? ReCaptchaResponse { get; set; }
    }
}
