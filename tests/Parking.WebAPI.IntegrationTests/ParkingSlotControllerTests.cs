using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Parking.WebAPI.DTOs;
using Xunit;

namespace Parking.WebAPI.IntegrationTests
{
    public class ParkingSlotControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ParkingSlotControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAllParkingSlots_ReturnsOkAndJsonArray()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/parkingslots");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var slots = await response.Content.ReadFromJsonAsync<List<ParkingSlotResponseDTO>>();
            slots.Should().NotBeNull();
            // Because our DatabaseSeeder runs automatically on startup, 
            // we expect the 20 seeded dummy slots to be returned.
            slots.Should().HaveCount(20);
        }

        [Fact]
        public async Task GetParkingSlotById_ValidId_ReturnsOkAndSlotData()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Fetch all slots first to dynamically grab a valid, seeded ID
            var allSlots = await client.GetFromJsonAsync<List<ParkingSlotResponseDTO>>("/api/parkingslots");
            var validSlot = allSlots![0];

            // Act
            var response = await client.GetAsync($"/api/parkingslots/{validSlot.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var slot = await response.Content.ReadFromJsonAsync<ParkingSlotResponseDTO>();
            slot.Should().NotBeNull();
            slot!.Id.Should().Be(validSlot.Id);
            slot.Designation.Should().Be(validSlot.Designation);
        }

        [Fact]
        public async Task GetParkingSlotById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var invalidId = Guid.NewGuid();

            // Act
            var response = await client.GetAsync($"/api/parkingslots/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}