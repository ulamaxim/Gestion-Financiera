namespace Gestion_Financiera.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public int BankId { get; set; }
        public int ConectionId { get; set; }
        public string? ExternalAccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty; // 'Ahorros', 'Corriente', 'TarjetaCredito', 'Inversion'
        public string Currency { get; set; } = "EUR";
        public decimal Balance { get; set; } = 0.0000m;
        public bool AccountStatus { get; set; } = true;
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación (Opcionales)
        public BankInfo? BankInfo { get; set; }
    }
}
