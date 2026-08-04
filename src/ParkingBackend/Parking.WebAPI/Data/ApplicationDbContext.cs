using Microsoft.EntityFrameworkCore;
using Parking.WebAPI.Models;
using System.Text.Json;

namespace Parking.WebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                    : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ParkingSlot> ParkingSlots { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enforce required configurations and relationships
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.ParkingSlot)
                .WithMany(p => p.Reservations)
                .HasForeignKey(r => r.ParkingSlotId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure Designation is unique (e.g., no two "A1" slots)
            modelBuilder.Entity<ParkingSlot>()
                .HasIndex(p => p.Designation)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.AuthorizedSlotTypes)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<SlotType>>(v, (JsonSerializerOptions)null) ?? new List<SlotType>()
                );
        }
    }
}
