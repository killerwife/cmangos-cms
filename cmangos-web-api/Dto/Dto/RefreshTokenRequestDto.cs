using System.ComponentModel.DataAnnotations;

namespace Data.Dto
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
