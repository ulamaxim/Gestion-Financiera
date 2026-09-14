namespace Gestion_Financiera.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public long TransferOriginId { get; set; } // Referencia a TransactionId (BIGINT)
        public long TransferEndId { get; set; }    // Referencia a TransactionId (BIGINT)
        public decimal Amount { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación (Opcionales)
        public Transaction? TransferOrigin { get; set; }
        public Transaction? TransferEnd { get; set; }
    }
}
