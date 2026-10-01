using Dapper;
using Gestion_Financiera.Data;
using Gestion_Financiera.Models.DTOs;
using Gestion_Financiera.Repositories.Interfaces;

namespace Gestion_Financiera.Repositories.Implementations;

public class BudgetRepository : IBudgetRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public BudgetRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<BudgetProgressDTO>> GetBudgetProgressAsync(int userId, int month, int year)
    {
        using var db = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT 
                b.CategoryId,
                c.CategoryName,
                b.LimitAmount,
                ISNULL(SUM(ABS(t.Amount)), 0) AS SpentAmount
            FROM Budget b
            INNER JOIN Category c ON b.CategoryId = c.CategoryId
            LEFT JOIN Transactions t ON t.CategoryId = b.CategoryId 
                AND t.UserId = b.UserId 
                AND MONTH(t.TransactionDate) = @Month 
                AND YEAR(t.TransactionDate) = @Year
                AND t.Amount < 0
            WHERE b.UserId = @UserId
            GROUP BY b.CategoryId, c.CategoryName, b.LimitAmount;";

        return await db.QueryAsync<BudgetProgressDTO>(sql, new { UserId = userId, Month = month, Year = year });
    }

    public async Task<bool> SaveBudgetAsync(int userId, int categoryId, decimal limitAmount)
    {
        using var db = _connectionFactory.CreateConnection();
        const string sql = @"
            IF EXISTS (SELECT 1 FROM Budget WHERE UserId = @UserId AND CategoryId = @CategoryId)
            BEGIN
                UPDATE Budget 
                SET LimitAmount = @LimitAmount 
                WHERE UserId = @UserId AND CategoryId = @CategoryId;
            END
            ELSE
            BEGIN
                INSERT INTO Budget (UserId, CategoryId, LimitAmount) 
                VALUES (@UserId, @CategoryId, @LimitAmount);
            END";

        int rowsAffected = await db.ExecuteAsync(sql, new { UserId = userId, CategoryId = categoryId, LimitAmount = limitAmount });
        return rowsAffected > 0;
    }
}