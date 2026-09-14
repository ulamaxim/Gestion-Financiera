namespace Gestion_Financiera.Models
{
    public class Budget
    {
        public int BudgetId { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public decimal MontoLimite { get; set; }
        public string Periodo { get; set; } = "Mensual"; // 'Mensual', 'Anual', 'Personalizado'
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación (Opcionales)
        public Category? Category { get; set; }
    }
}
