using Gestion_Financiera.Models.DTOs;

namespace Gestion_Financiera.Repositories.Interfaces;

public interface IFinancialMetricsRepository
{
    Task<IEnumerable<CategoryExpensesDTO>> GetGastosPorCategoriaAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin);
    Task<IEnumerable<MonthlyTrendDTO>> GetEvolucionMensualAsync(int usuarioId, int mesesAtras);
}
