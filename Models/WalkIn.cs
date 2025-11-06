using System.ComponentModel.DataAnnotations;

namespace StretchFitnessHub.Models
{
    public class WalkIn
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        public DateTime TimeIn { get; set; }

        [Required]
        [MaxLength(50)]
        public string Plan { get; set; }  

        [MaxLength(100)]
        public string? PreferredClass { get; set; }
        public bool IsApprove { get; set; } = false;
    }
}
