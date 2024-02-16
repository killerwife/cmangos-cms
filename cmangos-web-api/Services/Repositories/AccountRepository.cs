using cmangos_web_api.Repositories;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Services.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private CmangosDbContext _dbContext;

        public AccountRepository(CmangosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Account?> FindByUsername(string userName)
        {
            var query = _dbContext.Accounts.Where(p => p.username == userName).SingleOrDefault();
            return query;
        }
    }
}
