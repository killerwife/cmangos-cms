using cmangos_web_api.Repositories;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Services.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private RealmdDbContext _dbContext;
        private CmsDbContext _cmsContext;

        public AccountRepository(RealmdDbContext dbContext, CmsDbContext cmsContext)
        {
            _dbContext = dbContext;
            _cmsContext = cmsContext;
        }

        public async Task<Account> FindByToken(string reqRefreshToken)
        {
            var userId = _cmsContext.RefreshToken.Where(p =>p.Token == reqRefreshToken).Select(p => p.UserId).SingleOrDefault();
            return await Get(userId);
        }

        public async Task<Account?> FindByUsername(string userName)
        {
            var user = _dbContext.Accounts.Where(p => p.username == userName).SingleOrDefault();
            return user;
        }

        public async Task<Account?> Get(uint userId)
        {
            var user = _dbContext.Accounts.Where(p => p.id == userId).SingleOrDefault();
            return user;
        }

        public List<string>? GetRoles(uint userId)
        {
            var user = _dbContext.Accounts.Where(p => p.id == userId).SingleOrDefault();
            if (user == null)
                return null;

            return user.GetRoles();
        }

        public async Task<List<RefreshToken>> GetTokens(uint userId)
        {
            return _cmsContext.RefreshToken.Where(p => p.UserId == userId).ToList();
        }

        public async Task<bool> RevokeAndAddToken(RefreshToken token)
        {
            await RevokeTokens(token.UserId);
            _cmsContext.Add(token);
            var result = await _cmsContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> RevokeTokens(uint userId)
        {
            return _cmsContext.Database.ExecuteSql($"UPDATE refresh_token SET IsExpired=1 WHERE UserId={userId}") > 0; 
        }

        public async Task<bool> SetAuthenticatorToken(uint userId, string token)
        {
            var user = _dbContext.Accounts.Where(p => p.id == userId).SingleOrDefault();
            if (user == null)
                return false;

            user.token = token;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public Task<bool> UpdateToken(RefreshToken token)
        {
            throw new NotImplementedException();
        }
    }
}
