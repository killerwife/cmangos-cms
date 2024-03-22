using Services.Services;
using Data.Dto;
using Data.Dto.PlainAuth;
using Data.Dto.Srp6;
using Microsoft.AspNetCore.Mvc;
using cmangos_web_api.Helpers;
using Data.Dto.User;
using Microsoft.Extensions.Options;
using Configs;
using cmangos_web_api.ReCaptcha;

namespace cmangos_web_api.Controllers
{
    public class AuthorizationController : ControllerBase
    {
        private IAuthService _authService;
        private IOptionsMonitor<WebsiteConfig> _websiteOptions;

        public AuthorizationController(IAuthService authService, IOptionsMonitor<WebsiteConfig> websiteOptions)
        {
            _authService = authService;
            _websiteOptions = websiteOptions;
        }

        /// <summary>
        /// Requests srp6 challenge for login
        /// TODO
        /// </summary>
        /// <param name="authenticateRequest"></param>
        /// <response code="200"></response>
        /// <response code="400">Invalid username</response>
        [HttpPost("srp6/authorize")]
        public IActionResult Authorize([FromBody] AuthenticateRequestDto authenticateRequest)
        {
            return Ok();
        }

        /// <summary>
        /// Processes srp6 validation response
        /// TODO
        /// </summary>
        /// <param name="proof"></param>
        /// <response code="200"></response>
        /// <response code="400">Unsuccessful validation</response>
        [HttpPost("srp6/proof")]
        public IActionResult Proof([FromBody] ProofDto proof)
        {
            return Ok();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="register"></param>
        /// <response code="200"></response>
        /// <response code="400"></response>
        [HttpPost("srp6/registerChallenge")]
        public async Task<IActionResult> RegisterChallenge([FromBody] RegisterDto register)
        {
            // TODO
            return Ok();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="register"></param>
        /// <response code="200"></response>
        /// <response code="400"></response>
        [HttpPost("srp6/register")]
        public async Task<IActionResult> RegisterSrp6([FromBody] RegisterDto register)
        {
            // TODO
            return Ok();
        }

        /// <summary>
        /// Authorizes user and returns jwt bearer token and refresh token
        /// </summary>
        /// <param name="authenticateRequest">Credentials for authentication</param>
        /// <response code="200">Authentication success and tokens returned</response>
        /// <response code="400">Authentication failed</response>
        [HttpPost("plain/authorize")]
        public async Task<ActionResult<AuthResDto>> PlainAuthorize([FromBody] AuthenticatePlainRequestDto authenticateRequest)
        {
            var result = await _authService.ValidatePlainLogin(authenticateRequest.Name, authenticateRequest.Password, authenticateRequest.Pin);
            if (result.Errors != null)
                return BadRequest(result.Errors);
            return Ok(result);
        }

        /// <summary>
        /// Checks refresh token and returns a new bearer and refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token for new bearer</param>
        /// <response code="200">Authentication success and tokens returned</response>
        /// <response code="400">Authentication failed</response>
        [HttpPost("connect/token")]
        public async Task<ActionResult<AuthResDto>> RefreshToken([FromBody] RefreshTokenRequestDto refreshToken)
        {
            var result = await _authService.RefreshToken(refreshToken.RefreshToken, HttpContext.Connection.RemoteIpAddress!.ToString());
            if (result.Errors != null)
                return BadRequest(result.Errors);
            return Ok(result);
        }

        /// <summary>
        /// Revokes refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token to be revoked</param>
        /// <response code="200">Token successfully revoked</response>
        /// <response code="400">Could not revoke token</response>
        [HttpPost("revoke/token")]
        public async Task<ActionResult<AuthResDto>> RevokeToken([FromBody] RefreshTokenRequestDto refreshToken)
        {
            var result = await _authService.RevokeToken(refreshToken.RefreshToken, HttpContext.Connection.RemoteIpAddress!.ToString());
            return result == true ? Ok() : BadRequest();
        }

        /// <summary>
        /// Registers new user using provided credentials
        /// </summary>
        /// <param name="register">Username, password and email for registration</param>
        /// <response code="200">Registration success and verification email sent</response>
        /// <response code="400">Registration failed</response>
        /// <response code="409">Conflicting username</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            if (!string.IsNullOrEmpty(_websiteOptions.CurrentValue.ReCaptchaSecret))
            {
                var success = ReCaptchaHelper.ReCaptchaPassed(register.ReCaptchaResponse);
                if (success == false)
                    return BadRequest("Recaptcha failed");
            }
            IActionResult? result = await _authService.CreateAccount(register.Username, register.Password, register.Email, $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}");
            return result == null ? Ok() : result;
        }

        /// <summary>
        /// Verifies email using token sent to email address
        /// </summary>
        /// <param name="token">Token received in email</param>
        /// <response code="200">Verification success</response>
        /// <response code="400">Token could not be verified</response>
        [HttpGet("verifyemail/{token}")]
        public async Task<IActionResult> VerifyEmail([FromRoute] string token)
        {
            bool result = await _authService.VerifyEmail(token);
            if (result == true)
                Redirect(_websiteOptions.CurrentValue.VerifyEmailUrl);
            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Resends verification email to same email address with same token
        /// </summary>
        /// <response code="200">Email was sent successfully</response>
        /// <response code="400">No verification in progress</response>
        [Authorize]
        [HttpGet("resendverificationemail")]
        public async Task<IActionResult> ResendVerificationEmail()
        {
            var result = await _authService.ResendValidationEmail($"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}");
            return result == null ? Ok() : result;
        }

        /// <summary>
        /// Sends email with password reset token to associated user and stores token
        /// </summary>
        /// <param name="email">Email paired with account for password recovery</param>
        /// <response code="200">Email was successfully sent</response>
        /// <response code="400">User associated with email does not exist or request came too soon</response>
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var result = await _authService.ForgotPassword(email, $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}");
            return result == Repositories.PasswordRecoveryTokenResult.Success ? Ok() : BadRequest();
        }

        /// <summary>
        /// Requests password change using email sent token
        /// </summary>
        /// <param name="passwordRecovery">New password and password request token from email</param>
        /// <response code="200">New password set for user</response>
        /// <response code="400">Token for recovery was invalid</response>
        [HttpPost("passwordrecovery")]
        public async Task<IActionResult> PasswordRecovery([FromBody] PasswordRecoveryDto passwordRecovery)
        {
            var result = await _authService.PasswordRecovery(passwordRecovery.NewPassword, passwordRecovery.Token);
            return result == true ? Ok() : BadRequest();
        }

        /// <summary>
        /// Changes password for user
        /// </summary>
        /// <param name="changePassword">Old and new password for validation and change</param>
        /// <response code="200">Change success</response>
        /// <response code="400">Change password failed - old password invalid</response>
        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePassword)
        {
            var result = await _authService.ChangePassword(changePassword.Password, changePassword.NewPassword);
            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Generates secret for 2fa token and sends it to client
        /// </summary>
        /// <response code="200">Pending secret successfully added and returned</response>
        /// <response code="400">Save was not successful</response>
        [Authorize]
        [HttpGet("generatetokensecret")]
        public async Task<ActionResult<string>> GenerateTokenSecret()
        {
            var tokenSecret = await _authService.AddPendingAuthenticator();
            if (tokenSecret == null)
                return BadRequest();
            return Ok(tokenSecret);
        }

        /// <summary>
        /// Validates token against pending secret
        /// </summary>
        /// <param name="token">Token for validation against secret</param>
        /// <response code="200">Token was validated against pending secret and set as main</response>
        /// <response code="400">Token validation was not successful against pending secret</response>
        [Authorize]
        [HttpPost("addauthenticator")]
        public async Task<IActionResult> AddToken([FromBody] AddGoogleAuthenticatorDto token)
        {
            bool result = await _authService.AddAuthenticator(token.Token);
            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Removes authenticator from account
        /// </summary>
        /// <param name="token">Token for validation against secret</param>
        /// <response code="200">Token was validated against pending secret and removed from account</response>
        /// <response code="400">Authenticator removal was not successful</response>
        [Authorize]
        [HttpPost("removeauthenticator")]
        public async Task<IActionResult> RemoveToken([FromBody] AddGoogleAuthenticatorDto token)
        {
            bool result = await _authService.RemoveAuthenticator(token.Token);
            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Requests user information
        /// </summary>
        /// <response code="200">User information</response>
        /// <response code="401">Unauthorized</response>
        [Authorize]
        [HttpGet("userinfo")]
        public async Task<ActionResult<UserInfoDto>> GetUserInfo()
        {
            return Ok(await _authService.GetUserInfo());
        }
    }
}
