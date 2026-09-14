namespace Gestion_Financiera.Models
{
    public class Users
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool UserStatus { get; set; } = true;
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }

}
