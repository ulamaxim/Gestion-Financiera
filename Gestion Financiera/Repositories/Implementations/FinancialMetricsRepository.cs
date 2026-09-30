using Dapper;
using Gestion_Financiera.Data;
using Gestion_Financiera.Models.DTOs;
using Gestion_Financiera.Repositories.Interfaces;

namespace Gestion_Financiera.Repositories.Implementations;

public class FinancialMetricsRepository : IFinancialMetricsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public FinancialMetricsRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<CategoryExpensesDTO>> GetGastosPorCategoriaAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin)
    {
        using var db = _connectionFactory.CreateConnection();
        string sql = @"
            SELECT 
                ISNULL(c.CategoryName, 'Sin Categoría') AS CategoryName,
                SUM(ABS(t.Amount)) AS TotalAmount
            FROM Transactions t
            LEFT JOIN Category c ON t.CategoryId = c.CategoryId
            WHERE t.UserId = @UsuarioId 
              AND t.Amount < 0 
              AND t.TransactionDate >= @FechaInicio AND t.TransactionDate <= @FechaFin
            GROUP BY c.CategoryName
            ORDER BY TotalAmount DESC;";

        return await db.QueryAsync<CategoryExpensesDTO>(sql, new { UsuarioId = usuarioId, FechaInicio = fechaInicio, FechaFin = fechaFin });
    }

    public async Task<IEnumerable<MonthlyTrendDTO>> GetEvolucionMensualAsync(int usuarioId, int mesesAtras)
    {
        using var db = _connectionFactory.CreateConnection();
        string sql = @"
            SELECT 
                FORMAT(t.TransactionDate, 'MMM yyyy') AS MonthName,
                YEAR(t.TransactionDate) AS Year,
                MONTH(t.TransactionDate) AS Month,
                SUM(CASE WHEN t.Amount > 0 THEN t.Amount ELSE 0 END) AS TotalIncome,
                SUM(CASE WHEN t.Amount < 0 THEN ABS(t.Amount) ELSE 0 END) AS TotalExpenses
            FROM Transactions t
            WHERE t.UserId = @UsuarioId 
              AND t.TransactionDate >= DATEADD(MONTH, -@MesesAtras, GETDATE())
            GROUP BY FORMAT(t.TransactionDate, 'MMM yyyy'), YEAR(t.TransactionDate), MONTH(t.TransactionDate)
            ORDER BY Year ASC, Month ASC;";

        return await db.QueryAsync<MonthlyTrendDTO>(sql, new { UsuarioId = usuarioId, MesesAtras = mesesAtras });
    }
}
