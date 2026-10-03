using JobSequenceApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobSequenceApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
        public DbSet<WorkerProfile> WorkerProfiles { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<JobSequence> JobSequences { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure one-to-one relationships
            builder.Entity<ApplicationUser>()
                .HasOne(a => a.CustomerProfile)
                .WithOne(c => c.ApplicationUser)
                .HasForeignKey<CustomerProfile>(c => c.ApplicationUserId);

            builder.Entity<ApplicationUser>()
                .HasOne(a => a.WorkerProfile)
                .WithOne(w => w.ApplicationUser)
                .HasForeignKey<WorkerProfile>(w => w.ApplicationUserId);

            builder.Entity<JobSequence>()
                .HasOne(j => j.WorkerProfile)
                .WithMany(w => w.JobSequences)
                .HasForeignKey(j => j.WorkerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSequence>()
                .HasOne(j => j.Booking)
                .WithMany()
                .HasForeignKey(j => j.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.WorkerProfile)
                .WithMany()
                .HasForeignKey(b => b.WorkerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.WorkerProfile)
                .WithMany(w => w.Reviews)
                .HasForeignKey(r => r.WorkerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.CustomerProfile)
                .WithMany()
                .HasForeignKey(r => r.CustomerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Booking)
                .WithMany()
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WorkerProfile>()
                .Property(w => w.Pricing)
                .HasPrecision(18, 2);
        }
    }
}
