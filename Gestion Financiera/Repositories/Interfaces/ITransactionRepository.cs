namespace Gestion_Financiera.Repositories.Interfaces;

using Gestion_Financiera.Models;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId, int limit = 50);
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId);
    Task<long> CreateAsync(Transaction transaction);
    Task<bool> DeleteAsync(long transactionId);
}

