namespace cmangos_web_api.Auth
{
    public class UserInfo
    {
        /// <summary>
        /// User's Db key, initially migrated from original PP Atom
        /// </summary>
        public uint Id { get; }
        public string[] Roles { get; }

        internal UserInfo(uint id, string[] roles)
        {
            Id = id;
            Roles = roles;
        }

        public override string ToString()
        {
            return $"Id: {Id}";
        }

        public bool HasRole(string role)
        {
            return Roles.Contains(role);
        }
    }
}
