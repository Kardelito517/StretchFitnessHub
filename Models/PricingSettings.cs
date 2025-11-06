namespace StretchFitnessHub.Models
{
    public class PricingSettings
    {
        public int Id { get; set; }

        // Monthly Memberships
        public decimal GymOnlyPrice { get; set; }
        public decimal GymWithClassPrice { get; set; }
        public decimal ClassOnlyPrice { get; set; }

        // Walk-ins
        public decimal WalkInGymPrice { get; set; }
        public decimal WalkInClassPrice { get; set; }
    }
}
