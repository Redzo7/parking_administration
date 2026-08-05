using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Parking.WebAPI.Data;
using Parking.WebAPI.DTOs;
using Parking.WebAPI.Models;
using Xunit;

namespace Parking.WebAPI.IntegrationTests
{
    public class ReservationControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ReservationControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        // Helper method to retrieve valid seeded IDs directly from the database context
        private (Guid UserId, Guid SlotId) GetValidIds()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = dbContext.Users.First().Id;
            var slotId = dbContext.ParkingSlots.First(s => s.Type == SlotType.Regular).Id;

            return (userId, slotId);
        }

        [Fact]
        public async Task PostReservation_WithValidData_ReturnsCreated()
        {
            // Arrange
            var client = _factory.CreateClient();
            var (userId, slotId) = GetValidIds();

            var request = new ReservationRequestDTO
            {
                ParkingSlotId = slotId,
                UserId = userId,
                // Use a far future date to avoid overlapping with other parallel tests
                StartTime = DateTime.UtcNow.AddDays(10),
                EndTime = DateTime.UtcNow.AddDays(10).AddHours(2)
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/reservations", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdReservation = await response.Content.ReadFromJsonAsync<ReservationResponseDTO>();
            createdReservation.Should().NotBeNull();
            createdReservation!.ParkingSlotId.Should().Be(slotId);
            createdReservation.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task PostReservation_WithUnauthorizedSlotType_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Find a user with only Regular access, and a VIP slot
            var regularUser = dbContext.Users.AsEnumerable().First(u => !u.AuthorizedSlotTypes.Contains(SlotType.VIP));
            var vipSlot = dbContext.ParkingSlots.AsEnumerable().First(s => s.Type == SlotType.VIP);

            var request = new ReservationRequestDTO
            {
                ParkingSlotId = vipSlot.Id,
                UserId = regularUser.Id,
                StartTime = DateTime.UtcNow.AddDays(15),
                EndTime = DateTime.UtcNow.AddDays(15).AddHours(2)
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/reservations", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorContent = await response.Content.ReadAsStringAsync();
            errorContent.Should().Contain("not authorized");
        }

        [Fact]
        public async Task PostReservation_WithOverlappingTime_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var (userId, slotId) = GetValidIds();

            var request = new ReservationRequestDTO
            {
                ParkingSlotId = slotId,
                UserId = userId,
                StartTime = DateTime.UtcNow.AddDays(11),
                EndTime = DateTime.UtcNow.AddDays(11).AddHours(2)
            };

            // Act
            // 1. Create the initial valid reservation
            await client.PostAsJsonAsync("/api/reservations", request);

            // 2. Try to create a second reservation for the exact same time and slot
            var overlappingResponse = await client.PostAsJsonAsync("/api/reservations", request);

            // Assert
            overlappingResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorContent = await overlappingResponse.Content.ReadAsStringAsync();
            errorContent.Should().Contain("booked");
        }

        [Fact]
        public async Task DeleteReservation_WithValidFutureId_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            var (userId, slotId) = GetValidIds();

            var request = new ReservationRequestDTO
            {
                ParkingSlotId = slotId,
                UserId = userId,
                StartTime = DateTime.UtcNow.AddDays(12),
                EndTime = DateTime.UtcNow.AddDays(12).AddHours(2)
            };

            var createResponse = await client.PostAsJsonAsync("/api/reservations", request);
            var createdReservation = await createResponse.Content.ReadFromJsonAsync<ReservationResponseDTO>();

            // Act
            // We must use HttpRequestMessage to inject the custom X-User-Id header
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/reservations/{createdReservation!.Id}");
            requestMessage.Headers.Add("X-User-Id", userId.ToString());

            var deleteResponse = await client.SendAsync(requestMessage);

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteReservation_WithUnauthorizedUser_ReturnsForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();
            var (ownerId, slotId) = GetValidIds();
            var hackerId = Guid.NewGuid(); // A different, unauthorized user ID

            var request = new ReservationRequestDTO
            {
                ParkingSlotId = slotId,
                UserId = ownerId,
                StartTime = DateTime.UtcNow.AddDays(13),
                EndTime = DateTime.UtcNow.AddDays(13).AddHours(2)
            };

            var createResponse = await client.PostAsJsonAsync("/api/reservations", request);
            var createdReservation = await createResponse.Content.ReadFromJsonAsync<ReservationResponseDTO>();

            // Act
            // Attempt deletion using the hacker's ID in the header
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/reservations/{createdReservation!.Id}");
            requestMessage.Headers.Add("X-User-Id", hackerId.ToString());

            var deleteResponse = await client.SendAsync(requestMessage);

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var errorContent = await deleteResponse.Content.ReadAsStringAsync();
            errorContent.Should().Contain("not authorized");
        }
    }
}