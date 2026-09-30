namespace Gestion_Financiera.Models.DTOs
{
    // Para Gráficos de Barras (Evolución Mensual)
    public class EvolucionMensualDTO
    {
        public string Mes { get; set; } = string.Empty; // ej: "Ene 2026"
        public decimal Ingresos { get; set; }
        public decimal Gastos { get; set; }
        public decimal BalanceNeto => Ingresos - Gastos;
    }
}
