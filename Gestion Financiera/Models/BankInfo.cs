namespace Gestion_Financiera.Models
{
    public class BankInfo
    {
        public int BankId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string ApiProviderCode { get; set; } = string.Empty;
        public string ApiProviderName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }
}
