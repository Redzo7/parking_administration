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
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UserControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetUsers_ReturnsOkAndExactly5SeededUsers()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/users");

            // Assert
            // 1. Test GET /api/users returns a 200 OK status code.
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // 2. Test that the response contains exactly 5 users.
            var users = await response.Content.ReadFromJsonAsync<List<UserResponseDTO>>();
            users.Should().NotBeNull();
            users.Should().HaveCount(5, "because the DatabaseSeeder injects exactly 5 dummy users on startup");
        }
    }
}