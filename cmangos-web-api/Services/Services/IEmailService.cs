using Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    internal interface IEmailService
    {
        Task<IActionResult?> SendToken(string username, string email, string verificationToken, string caleeUrl, string locale, Operation operation);
    }
}
