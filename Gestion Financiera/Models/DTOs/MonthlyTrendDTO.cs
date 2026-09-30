namespace Gestion_Financiera.Models.DTOs
{
    // Para Gráficos de Barras (Evolución Mensual)
    public class MonthlyTrendDTO
    {
        public string MonthName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
    }
}
