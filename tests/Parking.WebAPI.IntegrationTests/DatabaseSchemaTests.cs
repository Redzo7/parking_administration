using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Parking.WebAPI.Data;
using Parking.WebAPI.Models;
using Xunit;

namespace Parking.WebAPI.IntegrationTests
{
    public class DatabaseSchemaTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public DatabaseSchemaTests()
        {
            // Set up a persistent SQLite in-memory database connection
            // The connection must remain open; otherwise, the in-memory DB is destroyed.
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;
        }

        [Fact]
        public async Task Database_ShouldApplyMigrations_AndSupportReadWriteOperations()
        {
            // --------------------------------------------------------
            // 1. Arrange & Apply Migrations
            // --------------------------------------------------------
            using var setupContext = new ApplicationDbContext(_options);

            // This applies the migrations directly to the in-memory SQLite database
            await setupContext.Database.MigrateAsync();

            // --------------------------------------------------------
            // 2. Act - Write Operations
            // --------------------------------------------------------
            var user = new User { Id = Guid.NewGuid(), Name = "Integration Test User" };
            var slot = new ParkingSlot { Id = Guid.NewGuid(), Designation = "T-01" };
            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                UserId = user.Id,
                ParkingSlotId = slot.Id
            };

            setupContext.Users.Add(user);
            setupContext.ParkingSlots.Add(slot);
            setupContext.Reservations.Add(reservation);

            var writtenEntries = await setupContext.SaveChangesAsync();

            // Assert write operation modified 3 entries
            writtenEntries.Should().Be(3);

            // --------------------------------------------------------
            // 3. Act - Read Operations
            // --------------------------------------------------------
            // Use a new context instance to ensure we read from the DB, not from EF's local cache
            using var readContext = new ApplicationDbContext(_options);

            var savedUser = await readContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            var savedSlot = await readContext.ParkingSlots.FirstOrDefaultAsync(p => p.Id == slot.Id);
            var savedReservation = await readContext.Reservations
                .Include(r => r.User)
                .Include(r => r.ParkingSlot)
                .FirstOrDefaultAsync(r => r.Id == reservation.Id);

            // --------------------------------------------------------
            // 4. Assert - Read Operations
            // --------------------------------------------------------
            savedUser.Should().NotBeNull();
            savedUser!.Name.Should().Be("Integration Test User");

            savedSlot.Should().NotBeNull();
            savedSlot!.Designation.Should().Be("T-01");

            savedReservation.Should().NotBeNull();
            savedReservation!.User.Should().NotBeNull();
            savedReservation.ParkingSlot.Should().NotBeNull();
            savedReservation.ParkingSlot!.Designation.Should().Be("T-01");
        }

        public void Dispose()
        {
            // Clean up the connection to destroy the in-memory database
            _connection.Close();
            _connection.Dispose();
        }
    }
}