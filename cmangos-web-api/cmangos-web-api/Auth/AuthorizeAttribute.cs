using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Common;

namespace cmangos_web_api.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public string? Policy { get; set; }
        public string? Claim { get; set; }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            ClaimsPrincipal? user = context.HttpContext.User;
            //var user = context.HttpContext.User;

            bool success = true;
            if (user == null)
                success = false;
            else if (Claim != null && user.Claims.Where(p => p.Value == Claim).SingleOrDefault() != null)
                success = false;
            else if (Policy != null && !HasPolicyClaims(user.Claims))
                success = false;
            if (!success)
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }

        private bool HasPolicyClaims(IEnumerable<Claim> claims)
        {
            var policyClaims = Constants.Mapping[Policy!];
            foreach (var policyClaim in policyClaims)
            {
                if (claims.Where(p => p.Value == policyClaim).SingleOrDefault() != null)
                    return true;
            }
            return false;
        }
    }
}