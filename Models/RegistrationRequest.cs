namespace StretchFitnessHub.Models
{
    public class RegistrationRequest
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string MembershipPlan { get; set; } = string.Empty;
        public string? PreferredClasses { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;
    }
}
