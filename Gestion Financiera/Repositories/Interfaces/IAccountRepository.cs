using Gestion_Financiera.Models;

namespace Gestion_Financiera.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetByUserIdAsync(int userId);
        Task<Account?> GetByIdAsync(int accountId);
        Task<int> CreateAsync(Account account);
        Task<bool> UpdateBalanceAsync(int accountId, decimal newBalance);
    }
}