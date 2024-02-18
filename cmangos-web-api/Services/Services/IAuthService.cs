using Data.Dto;
using Data.Dto.Srp6;
using System.Security.Claims;

namespace cmangos_web_api.Services
{
    public interface IAuthService
    {
        Task<ChallengeResponseDto> GenerateChallenge(string email);
        Task<AuthResDto> ValidateProof(byte[] answer, byte[] m1, byte[] crcHash, byte[] pin);
        Task<AuthResDto> ValidatePlainLogin(string name, string password, string? pin);
        string CreateToken(List<Claim> claims);
        IEnumerable<Claim> DecodeToken(string token);
        Task<AuthResDto?> RefreshToken(string token, string ipAddress);
        Task<bool> RevokeToken(string token, string ipAddress);

        Task<string?> AddPendingAuthenticator();
        Task<bool> AddAuthenticator(string token);

        Task<bool> VerifyEmail(string token);
    }
}
