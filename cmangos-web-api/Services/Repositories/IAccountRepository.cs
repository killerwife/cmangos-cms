using Data.Model;
using System.Security.Principal;

namespace cmangos_web_api.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> FindByToken(string reqRefreshToken);
        Task<Account?> FindByUsername(string userName);
        Task<Account?> Get(uint userId);
        Task<AccountExtension?> GetExt(uint userId);
        List<string>? GetRoles(uint userId);
        Task<bool> SetAuthenticatorToken(uint userId, string token);
        Task<List<RefreshToken>> GetTokens(uint userId);
        Task<bool> UpdateToken(RefreshToken token);
        Task<bool> RevokeAndAddToken(RefreshToken token);
        Task<bool> RevokeTokens(uint userId);
        Task<bool> AddPendingAuthenticator(uint userId, string token);
        Task<bool> QualifyPendingToken(AccountExtension ext);
        Task<bool> VerifyPendingEmail(string token);
        Task<Account?> Create(Account account, string email, string confirmationToken);
    }
}
