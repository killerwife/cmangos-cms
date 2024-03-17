using Data.Model;
using System.Security.Principal;

namespace cmangos_web_api.Repositories
{
    public enum PasswordRecoveryTokenResult
    {
        Success = 0,
        Collision = 1,
        TooSoon = 2,
        NotFound = 3,
    }

    public interface IAccountRepository
    {
        Task<Account?> FindByToken(string reqRefreshToken);
        Task<Account?> FindByUsername(string userName);
        Task<Account?> Get(uint userId);
        Task<Account?> FindByEmail(string email);
        Task<AccountExtension?> GetExt(uint userId);
        List<string>? GetRoles(uint userId);
        Task<bool> SetAuthenticatorToken(uint userId, string token);
        Task<List<RefreshToken>> GetTokens(uint userId);
        Task<bool> UpdateToken(RefreshToken token);
        Task<bool> RevokeAndAddToken(RefreshToken token);
        Task<bool> RevokeTokens(uint userId);
        Task<bool> AddPendingAuthenticator(uint userId, string token);
        Task<bool> QualifyPendingAuthenticator(AccountExtension ext);
        Task<bool> VerifyPendingEmail(string token);
        Task<Account?> Create(Account account, string email, string confirmationToken);
        Task CreateExtIfNotExists(uint userId);
        Task<(DateTime? lastSentTime, string? validationToken, string? email)?> GetEmailValidationData(uint userId);
        Task<bool> ChangePassword(Account account, AccountExtension? ext, string salt, string verifier);
        Task<PasswordRecoveryTokenResult> SetPasswordRecoveryToken(Account user, string v);
        Task<(AccountExtension? ext, Account? account)> FindByPasswordRecoveryToken(string token);
        Task<bool> RemoveAuthenticator(uint userId);
    }
}
