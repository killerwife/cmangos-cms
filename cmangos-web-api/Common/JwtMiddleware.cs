namespace cmangos_web_api.Helpers
{
    public class JwtMiddleware
    {
        /*
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthService authService, IUserRepository userRepository)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, authService, token, userRepository);

            if (context.Items["User"] != null)
                context.Response.OnStarting(state =>
                {
                    var httpContext = (HttpContext)state;
                    httpContext.Response.Headers.Remove("Cache-Control");
                    httpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    return Task.CompletedTask;
                }, context);

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, IAuthService authService, string token, IUserRepository userRepository)
        {
            try
            {
                var claims = authService.DecodeToken(token);
                if (claims == null)
                    return;
                var userId = claims.SingleOrDefault(p => p.Type == "sub");
                if (userId == null)
                    return;
                var user = userRepository.Get(userId.Value);
                if (user == null)
                    return;
                context.Items["User"] = user;
                context.Items["Claim"] = userRepository.GetClaims(user.Uuid);
            }
            catch (Exception)
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
        */
    }
}
