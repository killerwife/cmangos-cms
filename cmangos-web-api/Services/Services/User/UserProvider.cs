using System.Security.Claims;

namespace cmangos_web_api.Auth
{
    public class UserProvider : IUserProvider
    {
        private ClaimsPrincipal _principal;
        public UserInfo? CurrentUser { get; set; }
        public UserProvider(ClaimsPrincipal principal)
        {
            _principal = principal;
            CurrentUser = GetUser();
        }
        private UserInfo? GetUser()
        {
            if (_principal == null || _principal.Claims == null)
            {
                // log that principal is NULL, but do not fire exception app can continue..
                //throw new ArgumentNullException("Claims principal is null");
            }

            var id = _principal?.Claims?.Where(x => x.Type == "sub").FirstOrDefault()?.Value;
            var roles = _principal?.Claims?.Where(x => x.Type == "role").Select(p => p.Value);
            var name = _principal?.Claims?.Where(x => x.Type == "name").FirstOrDefault()?.Value;

            bool parseResult = uint.TryParse(id, out uint userId);
            if (parseResult == false || roles == null || name == null)
                return null;

            var result = new UserInfo(userId, roles.ToArray(), name);
            return result;
        }
    }
}
