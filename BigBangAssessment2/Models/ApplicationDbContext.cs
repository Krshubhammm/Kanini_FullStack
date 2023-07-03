using Microsoft.EntityFrameworkCore;

namespace BigBangAssessment2.Models
{
    public class ApplicationDbContext:DbContext
    {
       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasOne(b => b.Doctor)
                .WithMany(a => a.Patients)
                .HasForeignKey(p => p.Id);
        }
    }
}
