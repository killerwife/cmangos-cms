using cmangos_web_api.Repositories;
using Data.Dto;
using Data.Dto.Srp6;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Services.Services
{
    public interface IAuthService
    {
        Task<ChallengeResponseDto> GenerateChallenge(string email);
        Task<AuthResDto> ValidateProof(byte[] answer, byte[] m1, byte[] crcHash, byte[] pin);
        Task<AuthResDto> ValidatePlainLogin(string name, string password, string? pin);
        string CreateToken(List<Claim> claims);
        IEnumerable<Claim> DecodeToken(string token);
        Task<AuthResDto> RefreshToken(string token, string ipAddress);
        Task<bool> RevokeToken(string token, string ipAddress);

        Task<string?> AddPendingAuthenticator();
        Task<bool> AddAuthenticator(string token);

        Task<bool> VerifyEmail(string token);
        Task<IActionResult?> CreateAccount(string username, string password, string email, string url);
        Task<IActionResult?> ResendValidationEmail(string url);

        Task<PasswordRecoveryTokenResult> ForgotPassword(string email, string url);
        Task<bool> ChangePassword(string password, string newPassword);
        Task<bool> PasswordRecovery(string newPassword, string token);
    }
}
