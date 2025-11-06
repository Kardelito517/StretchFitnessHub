using System.ComponentModel.DataAnnotations;

namespace StretchFitnessHub.Models
{
    public class Staff
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = "";

        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        [EmailAddress]
        public string? Email { get; set; }

        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiration { get; set; }

        // Optional: Role field, default = "Staff"
        [Required]
        public string Role { get; set; } = "Staff";
    }
}
