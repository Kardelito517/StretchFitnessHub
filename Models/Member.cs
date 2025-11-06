namespace StretchFitnessHub.Models
{
    public class Member
    {
        public int Id { get; set; }

        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }

        public DateTime MembershipStart { get; set; }
        public DateTime MembershipEnd { get; set; }

        // ✅ Changed: MembershipType → MembershipPlan
        public string MembershipPlan { get; set; } = "GymOnly";

        // ✅ PreferredClasses becomes optional
        public string? PreferredClasses { get; set; }

        public bool IsArchived { get; set; } = false;
        public string Status { get; set; } = "Active";

        public DateTime? FreezeStartDate { get; set; }

        public string? ProfileImagePath { get; set; }

        public string QrCodeValue { get; set; } = string.Empty;
        public byte[]? QrCodeImage { get; set; }
    }
}
