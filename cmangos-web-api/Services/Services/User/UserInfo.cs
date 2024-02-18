namespace cmangos_web_api.Auth
{
    public class UserInfo
    {
        /// <summary>
        /// User's Db key, initially migrated from original PP Atom
        /// </summary>
        public uint Id { get; }
        public string Name { get; }
        public string[] Roles { get; }

        internal UserInfo(uint id, string[] roles, string name)
        {
            Id = id;
            Roles = roles;
            Name = name;
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
