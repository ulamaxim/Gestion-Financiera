namespace Gestion_Financiera.Models.DTOs
{
    // Para Presupuestos Programables
    public class BudgetProgressDTO
    {
        public int PresupuestoId { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public decimal LimiteMensual { get; set; }
        public decimal GastadoActual { get; set; }
        public decimal Restante => LimiteMensual - GastadoActual;
        public double PorcentajeConsumido => LimiteMensual > 0 ? (double)(GastadoActual / LimiteMensual) * 100 : 0;
    }
}
