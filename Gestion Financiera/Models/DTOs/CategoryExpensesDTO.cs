namespace Gestion_Financiera.Models.DTOs
{
    // Para el PieChart (Gastos por Categoría)
    public class CategoryExpensesDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public double Percentage { get; set; }
    }
}
