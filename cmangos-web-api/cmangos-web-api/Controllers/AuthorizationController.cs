﻿using Services.Services;
using Data.Dto;
using Data.Dto.PlainAuth;
using Data.Dto.Srp6;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtpNet;
using Microsoft.AspNetCore.Http.Extensions;

namespace cmangos_web_api.Controllers
{
    public class AuthorizationController : ControllerBase
    {
        private IAuthService _authService;

        public AuthorizationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("srp6/authorize")]
        public IActionResult Authorize([FromBody] AuthenticateRequestDto authenticateRequest)
        {
            return Ok();
        }

        [HttpPost("srp6/proof")]
        public IActionResult Proof([FromBody] ProofDto proof)
        {
            return Ok();
        }

        [HttpPost("plain/authorize")]
        public async Task<ActionResult<AuthResDto>> PlainAuthorize([FromBody] AuthenticatePlainRequestDto authenticateRequest)
        {
            var result = await _authService.ValidatePlainLogin(authenticateRequest.Name, authenticateRequest.Password, authenticateRequest.Pin);
            if (result.Errors != null)
                return BadRequest(result.Errors);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            IActionResult? result = await _authService.CreateAccount(register.Username, register.Password, register.Email, HttpContext.Request.GetEncodedUrl());
            return result == null ? Ok() : result;
        }

        [HttpGet("verifyemail/{token}")]
        public async Task<IActionResult> VerifyEmail([FromRoute] string token)
        {
            bool result = await _authService.VerifyEmail(token);
            // TODO: Add redirect configurable to frontend
            return result ? Ok() : BadRequest();
        }

        [Authorize]
        [HttpGet("resendVerificationEmail")]
        public async Task<IActionResult> ResendVerificationEmail()
        {
            // TODO
            return Ok();
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // TODO
            return Ok();
        }

        [Authorize]
        [HttpGet("generatetokensecret")]
        public async Task<ActionResult<string>> GenerateTokenSecret()
        {
            var tokenSecret = await _authService.AddPendingAuthenticator();
            if (tokenSecret == null)
                return BadRequest();
            return Ok(tokenSecret);
        }

        [Authorize]
        [HttpGet("addauthenticator")]
        public async Task<IActionResult> AddToken([FromBody] AddGoogleAuthenticatorDto token)
        {
            await _authService.AddAuthenticator(token.Token);
            return Ok();
        }
    }
}
