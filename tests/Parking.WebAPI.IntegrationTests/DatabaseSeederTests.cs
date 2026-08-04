using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Parking.WebAPI.Data;
using Xunit;

namespace Parking.WebAPI.IntegrationTests
{
    public class DatabaseSeederTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public DatabaseSeederTests()
        {
            // Set up an isolated SQLite in-memory database connection for this test class
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;
        }

        [Fact]
        public async Task SeedAsync_ShouldPopulateEmptyDatabase_WithDefaultUsersAndSlots()
        {
            // --------------------------------------------------------
            // 1. Arrange
            // --------------------------------------------------------
            using var context = new ApplicationDbContext(_options);

            // Ensure the schema is created (SeedAsync handles this, but it is good practice to note)
            // The database is guaranteed to be completely empty at this point due to the new in-memory connection.

            // --------------------------------------------------------
            // 2. Act
            // --------------------------------------------------------
            await DatabaseSeeder.SeedAsync(context);

            // --------------------------------------------------------
            // 3. Assert
            // --------------------------------------------------------
            var userCount = await context.Users.CountAsync();
            var slotCount = await context.ParkingSlots.CountAsync();

            userCount.Should().Be(5, "because the seeder should inject exactly 5 dummy users");
            slotCount.Should().Be(20, "because the seeder should inject exactly 20 dummy parking slots");
        }

        [Fact]
        public async Task SeedAsync_WhenCalledMultipleTimes_ShouldNotDuplicateData()
        {
            // --------------------------------------------------------
            // 1. Arrange
            // --------------------------------------------------------
            using var context = new ApplicationDbContext(_options);

            // --------------------------------------------------------
            // 2. Act (Call seeder twice)
            // --------------------------------------------------------
            await DatabaseSeeder.SeedAsync(context);
            await DatabaseSeeder.SeedAsync(context);

            // --------------------------------------------------------
            // 3. Assert
            // --------------------------------------------------------
            var userCount = await context.Users.CountAsync();
            var slotCount = await context.ParkingSlots.CountAsync();

            userCount.Should().Be(5, "because the seeder should be idempotent and not insert duplicates");
            slotCount.Should().Be(20, "because the seeder should be idempotent and not insert duplicates");
        }

        public void Dispose()
        {
            // Clean up the connection to destroy the in-memory database
            _connection.Close();
            _connection.Dispose();
        }
    }
}