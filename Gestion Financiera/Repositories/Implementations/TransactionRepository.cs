using Dapper;
using Gestion_Financiera.Data;
using Gestion_Financiera.Models;
using Gestion_Financiera.Repositories.Interfaces;
using System.Data;

namespace Gestion_Financiera.Repositories.Implementations;

public class TransactionRepository : ITransactionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TransactionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId, int limit = 50)
    {
        using IDbConnection db = _connectionFactory.CreateConnection();
        string sql = $@"
            SELECT TOP (@Limit) t.*, a.*, c.*
            FROM Transactions t
            INNER JOIN Accounts a ON t.AccountId = a.AccountId
            LEFT JOIN Category c ON t.CategoryId = c.CategoryId
            WHERE t.UserId = @UserId
            ORDER BY t.TransactionDate DESC;";

        // Multi-mapping de 3 tablas (Transaction -> Account, Category)
        return await db.QueryAsync<Transaction, Account, Category, Transaction>(
            sql,
            (transaction, account, category) =>
            {
                transaction.Account = account;
                transaction.Category = category;
                return transaction;
            },
            new { UserId = userId, Limit = limit },
            splitOn: "AccountId,CategoryId"
        );
    }

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId)
    {
        using IDbConnection db = _connectionFactory.CreateConnection();
        string sql = @"
            SELECT t.*, c.* 
            FROM Transactions t
            LEFT JOIN Category c ON t.CategoryId = c.CategoryId
            WHERE t.AccountId = @AccountId
            ORDER BY t.TransactionDate DESC;";

        return await db.QueryAsync<Transaction, Category, Transaction>(
            sql,
            (transaction, category) =>
            {
                transaction.Category = category;
                return transaction;
            },
            new { AccountId = accountId },
            splitOn: "CategoryId"
        );
    }

    public async Task<long> CreateAsync(Transaction transaction)
    {
        using IDbConnection db = _connectionFactory.CreateConnection();

        // Ejecutamos la inserción y la actualización del saldo dentro de una transacción de BD
        db.Open();
        using var dbTransaction = db.BeginTransaction();

        try
        {
            string sqlInsert = @"
                INSERT INTO Transactions (AccountId, UserId, CategoryId, ExternalTransactionId, Amount, Currency, TransactionDate, TransactionDescription, TransactionStatus, IsIgnored)
                VALUES (@AccountId, @UserId, @CategoryId, @ExternalTransactionId, @Amount, @Currency, @TransactionDate, @TransactionDescription, @TransactionStatus, @IsIgnored);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";

            long transactionId = await db.ExecuteScalarAsync<long>(sqlInsert, transaction, dbTransaction);

            // Actualizar el saldo de la cuenta asociada
            string sqlUpdateBalance = @"
                UPDATE Accounts 
                SET Balance = Balance + @Amount 
                WHERE AccountId = @AccountId;";

            await db.ExecuteAsync(sqlUpdateBalance, new { Amount = transaction.Amount, AccountId = transaction.AccountId }, dbTransaction);

            dbTransaction.Commit();
            return transactionId;
        }
        catch
        {
            dbTransaction.Rollback();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long transactionId)
    {
        using IDbConnection db = _connectionFactory.CreateConnection();
        string sql = "DELETE FROM Transactions WHERE TransactionId = @TransactionId;";
        int rowsAffected = await db.ExecuteAsync(sql, new { TransactionId = transactionId });
        return rowsAffected > 0;
    }
}