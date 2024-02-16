using Data.Model;
using System.Security.Principal;

namespace cmangos_web_api.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> FindByUsername(string userName);
        //Task Create(Account user);
        //Account? FindByToken(string token);

        //Account? Get(string uuid);
        //List<AccountWithRole>? GetAll();
        //List<RefreshToken> GetTokens(string uuid);
        //Task AddToken(RefreshToken token);
        //Task UpdateToken(RefreshToken token);
        //Task RemoveToken(RefreshToken token);
        //Task RevokeTokens(string uuid);
        //Task RevokeAndAddToken(RefreshToken token);
        //Task Update(Account user);
        //Task<bool> Delete(string uuid);
        //List<string>? GetClaims(string uuid);
    }
}
