using Microsoft.EntityFrameworkCore;
using StretchFitnessHub.Models;

namespace StretchFitnessHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Staff> Staffs { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<RegistrationRequest> RegistrationRequests { get; set; }
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        public DbSet<WalkIn> WalkIns { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PricingSettings> PricingSettings { get; set; }


    }
}
