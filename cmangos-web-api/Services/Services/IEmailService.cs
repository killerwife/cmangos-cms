using Common;
using Microsoft.AspNetCore.Mvc;

namespace Services.Services
{
    public interface IEmailService
    {
        Task<IActionResult?> SendToken(string username, string email, string verificationToken, string caleeUrl, string locale, Operation operation);
    }
}
