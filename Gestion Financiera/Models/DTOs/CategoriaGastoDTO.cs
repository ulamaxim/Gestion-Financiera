namespace Gestion_Financiera.Models.DTOs
{
    // Para el PieChart (Gastos por Categoría)
    public class CategoriaGastoDTO
    {
        public string Categoria { get; set; } = "Sin Categoría";
        public decimal Total { get; set; }
        public double Porcentaje { get; set; }
    }
}
