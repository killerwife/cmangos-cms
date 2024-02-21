namespace Common
{
    public class Constants
    {
        public const int RsaKeyLength = 2048;

        public static class Claims
        {
            public const string Moderator = "MODERATOR";
            public const string Admin = "ADMIN";
        }

        public static class Policies
        {
            public const string Moderation = "MODERATION";
            public const string RoleAssignment = "ROLE_ASSIGNMENT";
        }

        public static Dictionary<string, List<string>> Mapping = new Dictionary<string, List<string>>
        {
            { Policies.Moderation, new List<string>() { Claims.Moderator, Claims.Admin } },
            { Policies.RoleAssignment, new List<string>() { Claims.Admin } }
        };
    }

    public enum Operation
    {
        SendConfirmationEmail = 0,
        SendPasswordRecovery = 1,
    }
}
