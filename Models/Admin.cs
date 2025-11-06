using System;
using System.ComponentModel.DataAnnotations;

namespace StretchFitnessHub.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiration { get; set; }

        // ✅ Field para malaman kung Admin o Staff
        [Required]
        public string Role { get; set; } = "Admin"; // default = Admin
    }
}
