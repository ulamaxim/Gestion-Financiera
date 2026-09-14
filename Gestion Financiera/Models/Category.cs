namespace Gestion_Financiera.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        // Foreign Key: Usuario (nulo si es una categoría global/por defecto)
        public int? UserId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryType { get; set; } = string.Empty; // 'Ingreso', 'Gasto', 'Transferencia'
        public string? Icon { get; set; }

        // Foreign Key: Autorreferencia para subcategorías
        public int? FatherCategoryId { get; set; }

        // Propiedades de navegación (Opcionales para JOINs en Dapper)
        public Category? FatherCategory { get; set; }
    }
}
