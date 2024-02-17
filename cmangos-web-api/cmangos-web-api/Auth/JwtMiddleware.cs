﻿using cmangos_web_api.Repositories;
using cmangos_web_api.Services;
using Microsoft.AspNetCore.Http;
using Services.Repositories;

namespace cmangos_web_api.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthService authService, IAccountRepository accountRepository)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachUserToContext(context, authService, token, accountRepository);

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

        private async Task AttachUserToContext(HttpContext context, IAuthService authService, string token, IAccountRepository accountRepository)
        {
            try
            {
                var claims = authService.DecodeToken(token);
                if (claims == null)
                    return;
                var userId = claims.SingleOrDefault(p => p.Type == "sub");
                if (userId == null)
                    return;
                var user = await accountRepository.Get(uint.Parse(userId.Value));
                if (user == null)
                    return;
                context.Items["User"] = user;
                context.Items["roles"] = accountRepository.GetRoles(user.id);
            }
            catch (Exception)
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}