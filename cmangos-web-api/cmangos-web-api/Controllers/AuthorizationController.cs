using cmangos_web_api.Services;
using Data.Dto;
using Data.Dto.PlainAuth;
using Data.Dto.Srp6;
using Microsoft.AspNetCore.Mvc;

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
    }
}
