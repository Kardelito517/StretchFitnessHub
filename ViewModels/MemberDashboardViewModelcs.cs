namespace StretchFitnessHub.ViewModels
{
    public class MemberDashboardViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty; 
        public DateTime MembershipStart { get; set; }
        public DateTime MembershipEnd { get; set; }

        // ✅ Automatically computed Remaining Days (always non-negative)
        public int RemainingDays
        {
            get
            {
                // use DateTime.UtcNow to avoid local clock issues
                var today = DateTime.Now.Date;
                if (MembershipEnd < today)
                    return 0;

                var remaining = (MembershipEnd.Date - today).Days;
                return remaining < 0 ? 0 : remaining;
            }
        }

        // ✅ Automatically computed Percentage based on elapsed time
        public int Percentage
        {
            get
            {
                var total = (MembershipEnd.Date - MembershipStart.Date).TotalDays;
                var elapsed = (DateTime.Now.Date - MembershipStart.Date).TotalDays;

                if (total <= 0) total = 1;
                if (elapsed < 0) elapsed = 0;
                if (elapsed > total) elapsed = total;

                var percent = (elapsed / total) * 100;
                return (int)Math.Round(percent);
            }
        }

        public string? ProfileImagePath { get; set; }
        public string? QrCodeImage { get; set; }
        public int MemberId { get; set; }

    }
}
