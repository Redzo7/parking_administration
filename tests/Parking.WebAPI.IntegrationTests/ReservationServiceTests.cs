using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Parking.WebAPI.Data;
using Parking.WebAPI.DTOs;
using Parking.WebAPI.Models;
using Parking.WebAPI.Services;
using Xunit;

namespace Parking.WebAPI.IntegrationTests
{
    public class ReservationServiceTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _context;
        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new ApplicationDbContext(_options);
            _context.Database.Migrate(); // Ensure schema is created

            _service = new ReservationService(_context);
        }

        [Fact]
        public async Task CreateReservation_WhenStartTimeIsInThePast_ShouldThrowArgumentException()
        {
            // Arrange
            var request = new ReservationRequestDTO
            {
                ParkingSlotId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                StartTime = DateTime.UtcNow.AddHours(-1), // Past
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            // Act
            Func<Task> act = async () => await _service.CreateReservationAsync(request);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Reservation start time cannot be in the past.");
        }

        [Fact]
        public async Task CreateReservation_WhenDurationIsLessThan30Minutes_ShouldThrowArgumentException()
        {
            // Arrange
            var startTime = DateTime.UtcNow.AddHours(1);
            var request = new ReservationRequestDTO
            {
                ParkingSlotId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                StartTime = startTime,
                EndTime = startTime.AddMinutes(29) // Less than 30 mins
            };

            // Act
            Func<Task> act = async () => await _service.CreateReservationAsync(request);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Reservation duration must be at least 30 minutes.");
        }

        [Fact]
        public async Task CreateReservation_WithAdjacentReservations_ShouldNotTriggerOverlap()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var slotId = Guid.NewGuid();
            await SeedUserAndSlotAsync(userId, slotId);

            var baseTime = DateTime.UtcNow.AddDays(1);

            // Existing reservation: 10:00 to 11:00
            _context.Reservations.Add(new Reservation
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ParkingSlotId = slotId,
                StartTime = baseTime,
                EndTime = baseTime.AddHours(1)
            });
            await _context.SaveChangesAsync();

            var request = new ReservationRequestDTO
            {
                ParkingSlotId = slotId,
                UserId = userId,
                StartTime = baseTime.AddHours(1), // Starts exactly when the other ends (11:00)
                EndTime = baseTime.AddHours(2)    // Ends at 12:00
            };

            // Act
            var result = await _service.CreateReservationAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.StartTime.Should().Be(request.StartTime);

            var count = await _context.Reservations.CountAsync();
            count.Should().Be(2, "because the adjacent reservation should be saved alongside the original");
        }

        [Fact]
        public async Task CreateReservation_WithPartiallyOverlappingReservation_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var slotId = Guid.NewGuid();
            await SeedUserAndSlotAsync(userId, slotId);

            var baseTime = DateTime.UtcNow.AddDays(1);

            // Existing reservation: 10:00 to 11:00
            _context.Reservations.Add(new Reservation
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ParkingSlotId = slotId,
                StartTime = baseTime,
                EndTime = baseTime.AddHours(1)
            });
            await _context.SaveChangesAsync();

            var request = new ReservationRequestDTO
            {
                ParkingSlotId = slotId,
                UserId = userId,
                StartTime = baseTime.AddMinutes(30), // Starts at 10:30 (Overlap)
                EndTime = baseTime.AddMinutes(90)    // Ends at 11:30
            };

            // Act
            Func<Task> act = async () => await _service.CreateReservationAsync(request);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("The parking slot is already booked for the requested time period.");
        }

        [Fact]
        public async Task CreateReservation_WithValidData_ShouldSaveToDatabaseAndReturnDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var slotId = Guid.NewGuid();
            await SeedUserAndSlotAsync(userId, slotId);

            var request = new ReservationRequestDTO
            {
                ParkingSlotId = slotId,
                UserId = userId,
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow.AddHours(4)
            };

            // Act
            var response = await _service.CreateReservationAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.ParkingSlotId.Should().Be(slotId);
            response.UserId.Should().Be(userId);

            var dbEntity = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == response.Id);
            dbEntity.Should().NotBeNull();
            dbEntity!.StartTime.Should().Be(request.StartTime);
            dbEntity.EndTime.Should().Be(request.EndTime);
        }

        private async Task SeedUserAndSlotAsync(Guid userId, Guid slotId)
        {
            _context.Users.Add(new User { Id = userId, Name = "Test User" });
            _context.ParkingSlots.Add(new ParkingSlot { Id = slotId, Designation = "T01" });
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }
    }
}
