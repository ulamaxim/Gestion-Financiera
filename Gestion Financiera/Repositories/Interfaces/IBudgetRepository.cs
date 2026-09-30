using Gestion_Financiera.Models.DTOs;

namespace Gestion_Financiera.Repositories.Interfaces;

public interface IBudgetRepository
{
    Task<IEnumerable<BudgetProgressDTO>> GetBudgetProgressAsync(int userId, int month, int year);
    Task<bool> SaveBudgetAsync(int userId, int categoryId, decimal limitAmount);
}
