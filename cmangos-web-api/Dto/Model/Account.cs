using System.ComponentModel.DataAnnotations;

namespace Data.Model
{
    public class Account
    {
        [Key]
        public uint id { get; set; }
        public string username { get; set; } = string.Empty;
        public uint gmlevel { get; set; }
        public string? v { get; set; } = string.Empty;
        public string? s { get; set; } = string.Empty;
        public string lockedIp { get; set; } = string.Empty;
        public string? email { get; set; } = string.Empty;
        public string? token { get; set; } = string.Empty;
        public int locked { get; set; }

        public List<string> GetRoles()
        {
            var roles = new List<string>();
            if (gmlevel >= 1)
                roles.Add("role_moderator");
            if (gmlevel >= 2)
                roles.Add("role_gm");
            if (gmlevel >= 3)
                roles.Add("role_admin");
            return roles;
        }
    }
}
