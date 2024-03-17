using cmangos_web_api.Repositories;
using Data.Model;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<Account?> FindByToken(string reqRefreshToken)
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

        public async Task<AccountExtension?> GetExt(uint userId)
        {
            return _cmsContext.AccountsExt.Where(p => p.Id == userId).SingleOrDefault();
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

        public async Task<bool> AddPendingAuthenticator(uint userId, string token)
        {
            var ext = await GetExt(userId);
            ext!.PendingToken = token;
            _cmsContext.AccountsExt.Update(ext);
            var result = await _cmsContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> QualifyPendingAuthenticator(AccountExtension ext)
        {
            var user = await Get(ext.Id);
            user!.token = ext.PendingToken;
            ext.PendingToken = null;
            _dbContext.Accounts.Update(user);
            var result = await _dbContext.SaveChangesAsync();
            _cmsContext.AccountsExt.Update(ext);
            var result2 = await _cmsContext.SaveChangesAsync();
            return result > 0 && result2 > 0;
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
            var result = _cmsContext.Database.ExecuteSql($"DELETE FROM refresh_token WHERE UserId={userId} AND Expires <= NOW() - INTERVAL 1 DAY");
            return result > 0; 
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

        public async Task<bool> UpdateToken(RefreshToken token)
        {
            _cmsContext.RefreshToken.Update(token);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> VerifyPendingEmail(string token)
        {
            var ext = _cmsContext.AccountsExt.Where(p => p.PendingEmailToken == token).SingleOrDefault();
            if (ext == null || ext.PendingEmail == null)
                return false;

            var user = await Get(ext.Id);
            user!.email = ext.PendingEmail;
            user.locked = 0;
            ext.PendingEmail = null;
            ext.PendingEmailToken = null;
            ext.PendingEmailTokenSent = null;

            _dbContext.Accounts.Update(user);
            var result = await _dbContext.SaveChangesAsync();
            _cmsContext.AccountsExt.Update(ext);
            var result2 = await _cmsContext.SaveChangesAsync();
            return result > 0 && result2 > 0;
        }

        public async Task<Account?> Create(Account account, string email, string confirmationToken)
        {
            if (_cmsContext.AccountsExt.Where(p => p.PendingEmailToken == confirmationToken).SingleOrDefault() != null)
                return null;

            _dbContext.Add(account);
            await _dbContext.SaveChangesAsync();
            var ext = new AccountExtension()
            {
                Id = account.id,
                PendingEmail = email,
                PendingEmailToken = confirmationToken,
                PendingEmailTokenSent = DateTime.UtcNow,
                PasswordChanged = DateTime.UtcNow,
            };
            _cmsContext.Add(ext);
            await _cmsContext.SaveChangesAsync();
            return account;
        }

        public async Task<(DateTime? lastSentTime, string? validationToken, string? email)?> GetEmailValidationData(uint userId)
        {
            var ext = await GetExt(userId);
            if (ext == null)
                return null;

            return (ext.PendingEmailTokenSent, ext.PendingToken, ext.PendingEmail);
        }

        public async Task<bool> ChangePassword(Account account, AccountExtension? ext, string salt, string verifier)
        {
            account.s = salt;
            account.v = verifier;
            if (ext == null)
                ext = await GetExt(account.id);

            ext!.PasswordChanged = DateTime.UtcNow;
            ext.PasswordRecoveryToken = null;
            _dbContext.Accounts.Update(account);
            var result = await _dbContext.SaveChangesAsync();
            _cmsContext.AccountsExt.Update(ext);
            var result2 = await _cmsContext.SaveChangesAsync();
            return result > 0 && result2 > 0;
        }

        public async Task<Account?> FindByEmail(string email)
        {
            return _dbContext.Accounts.Where(p => p.email == email).SingleOrDefault();
        }

        public async Task<PasswordRecoveryTokenResult> SetPasswordRecoveryToken(Account user, string token)
        {
            if (_cmsContext.AccountsExt.Where(p => p.PasswordRecoveryToken == token).SingleOrDefault() != null)
                return PasswordRecoveryTokenResult.Collision;

            var ext = await GetExt(user.id);
            if (ext!.PasswordRecoverySent > DateTime.UtcNow.AddMinutes(-2))
                return PasswordRecoveryTokenResult.TooSoon;

            ext.PasswordRecoverySent = DateTime.UtcNow;
            ext.PasswordRecoveryToken = token;

            _cmsContext.AccountsExt.Update(ext);
            await _cmsContext.SaveChangesAsync();
            return PasswordRecoveryTokenResult.Success;
        }

        public async Task<(AccountExtension? ext, Account? account)> FindByPasswordRecoveryToken(string token)
        {
            var ext = _cmsContext.AccountsExt.Where(p => p.PasswordRecoveryToken == token).SingleOrDefault();
            if (ext == null)
                return (null, null);

            return (ext, await Get(ext.Id));
        }

        public async Task CreateExtIfNotExists(uint userId)
        {
            var ext = await GetExt(userId);
            if (ext == null)
            {
                _cmsContext.Add(new AccountExtension
                {
                    Id = userId,
                    EmailChanged = DateTime.UtcNow,
                    PasswordChanged = DateTime.UtcNow
                });
                await _cmsContext.SaveChangesAsync();
            }
        }

        public async Task<bool> RemoveAuthenticator(uint userId)
        {
            var result = await _dbContext.Database.ExecuteSqlAsync($"UPDATE account SET Token=NULL WHERE id = {userId}");
            return result > 0;
        }
    }
}
