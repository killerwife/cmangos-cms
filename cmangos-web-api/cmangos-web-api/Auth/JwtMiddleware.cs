using cmangos_web_api.Repositories;
using Services.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace cmangos_web_api.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, token);

            if (context.Items["User"] != null)
                context.Response.OnStarting(state =>
                {
                    var httpContext = (HttpContext)state;
                    httpContext.Response.Headers.Remove("Cache-Control");
                    httpContext.Response.Headers.Append("Cache-Control", "no-cache, no-store");
                    return Task.CompletedTask;
                }, context);

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var claims = jwt.Claims;
                if (claims == null)
                    return;
                var userId = claims.SingleOrDefault(p => p.Type == "sub");
                if (userId == null)
                    return;
                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
                context.User = principal;
                context.Items["userId"] = userId.Value;
            }
            catch (Exception)
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
