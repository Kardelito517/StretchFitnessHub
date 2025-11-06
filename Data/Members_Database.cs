using Microsoft.EntityFrameworkCore;
using StretchFitnessHub.Models;

namespace StretchFitnessHub.Data
{
    public class Members_Database : DbContext
    {
        public Members_Database(DbContextOptions<Members_Database> options): base(options)
        {
        }

        public DbSet<RegistrationRequest> RegistrationRequests { get; set; }
        public DbSet<Member> Members { get; set; }
    }
}
