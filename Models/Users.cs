using System.ComponentModel.DataAnnotations;

namespace FirstWebApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public string Name { get; set; } = "";
        public string Role { get; set; } = "";

        public string? Phone { get; set; }

        public string? Status { get; set; }

        public DateTime? DOB { get; set; }

        public DateTime Created_at { get; set; }

        public DateTime Updated_at { get; set; }
    }
}