namespace Gestion_Financiera.Models
{
    public class ApiConection
    {
        public int ConectionId { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public int BankId { get; set; }
        public string ExternalItemId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? FechaExpiracionToken { get; set; }
        public string TokenStatus { get; set; } = "Activo"; // 'Activo', 'Expirado', 'Error'
        public DateTime? LastSincronization { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación (Opcionales)
        public BankInfo? BankInfo { get; set; }
    }
}
