using Gestion_Financiera.Data;
using Gestion_Financiera.Models;
using Gestion_Financiera.Repositories.Interfaces;
using System.Data;
using Dapper;

namespace Gestion_Financiera.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AccountRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Account>> GetByUserIdAsync(int userId)
        {
            using IDbConnection db = _connectionFactory.CreateConnection();
            string sql = @"
            SELECT a.*, b.* 
            FROM Accounts a
            JOIN BankInfo b ON a.BankId = b.BankId
            WHERE a.UserId = @UserId AND a.AccountStatus = 1;";

            // Multi-mapping: mapeamos Account y su relación BankInfo
            return await db.QueryAsync<Account, BankInfo, Account>(
                sql,
                (account, bankInfo) =>
                {
                    account.BankInfo = bankInfo;
                    return account;
                },
                new { UserId = userId },
                splitOn: "BankId"
            );
        }

        public async Task<Account?> GetByIdAsync(int accountId)
        {
            using IDbConnection db = _connectionFactory.CreateConnection();
            string sql = "SELECT * FROM Accounts WHERE AccountId = @AccountId;";
            return await db.QueryFirstOrDefaultAsync<Account>(sql, new { AccountId = accountId });
        }

        public async Task<int> CreateAsync(Account account)
        {
            using IDbConnection db = _connectionFactory.CreateConnection();
            string sql = @"
            INSERT INTO Accounts (UserId, BankId, ConectionId, ExternalAccountId, AccountName, AccountType, Currency, Balance, AccountStatus)
            VALUES (@UserId, @BankId, @ConectionId, @ExternalAccountId, @AccountName, @AccountType, @Currency, @Balance, @AccountStatus);
            SELECT CAST(SCOPE_IDENTITY() as int);";

            return await db.ExecuteScalarAsync<int>(sql, account);
        }

        public async Task<bool> UpdateBalanceAsync(int accountId, decimal newBalance)
        {
            using IDbConnection db = _connectionFactory.CreateConnection();
            string sql = "UPDATE Accounts SET Balance = @Balance WHERE AccountId = @AccountId;";
            int rowsAffected = await db.ExecuteAsync(sql, new { AccountId = accountId, Balance = newBalance });
            return rowsAffected > 0;
        }
    }
}
