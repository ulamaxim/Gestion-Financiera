namespace Gestion_Financiera.Models
{
    public class Transaction
    {
        // IDENTITY BIGINT en SQL se mapea a long en C#
        public long TransactionId { get; set; }

        // Foreign Keys
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public int? CategoryId { get; set; } // Puede ser nulo por 'ON DELETE SET NULL'
        public string? ExternalTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EUR";
        public DateTime TransactionDate { get; set; }
        public string TransactionDescription { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = "Completado"; // 'Pendiente', 'Completado'
        public bool IsIgnored { get; set; } = false;
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación (Opcionales para JOINs en Dapper)
        public Account? Account { get; set; }
        public Category? Category { get; set; }
    }
}
