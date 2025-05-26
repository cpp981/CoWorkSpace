using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using CoWorkSpace.Api.Controllers;
using CoWorkSpace.Api.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace CoWorkSpace.Api.Tests.Controllers
{
    public class StatsControllerTests
    {
        private readonly Mock<IStatsService> _statsServiceMock;
        private readonly StatsController _controller;

        public StatsControllerTests()
        {
            _statsServiceMock = new Mock<IStatsService>();
            _controller = new StatsController(_statsServiceMock.Object);
        }

        private void SetupUser(string role, string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        #region SuperAdmin Tests

        [Fact]
        public async Task GetUserStats_SuperAdmin_ReturnsOk()
        {
            // Arrange
            SetupUser("SuperAdmin", "superadmin-1");
            var stats = new UserStats
            {
                TotalUsers = 100,
                UsersByRole = new Dictionary<string, int>
                {
                    { "SuperAdmin", 2 },
                    { "Admin", 10 },
                    { "Provider", 30 },
                    { "Client", 58 }
                }
            };
            _statsServiceMock.Setup(s => s.GetUserStats()).ReturnsAsync(stats);

            // Act
            var result = await _controller.GetUserStats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(okResult.Value));
            Assert.Equal(100, Convert.ToInt32(response["totalUsers"]));
            var byRole = JsonSerializer.Deserialize<Dictionary<string, int>>(response["byRole"].ToString());
            Assert.Equal(2, byRole["SuperAdmin"]);
        }

        [Fact]
        public async Task GetUserStats_SuperAdmin_ThrowsException_Returns500()
        {
            // Arrange
            SetupUser("SuperAdmin", "superadmin-1");
            _statsServiceMock.Setup(s => s.GetUserStats()).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetUserStats();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetRegistrationStats_SuperAdmin_ReturnsOk()
        {
            // Arrange
            SetupUser("SuperAdmin", "superadmin-1");
            var stats = new List<RegistrationStats>
            {
                new() { Month = "2025-01", Count = 10 },
                new() { Month = "2025-02", Count = 15 }
            };
            _statsServiceMock.Setup(s => s.GetRegistrationStats()).ReturnsAsync(stats);

            // Act
            var result = await _controller.GetRegistrationStats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(okResult.Value));
            var registrations = JsonSerializer.Deserialize<List<RegistrationStats>>(response["registrations"].ToString());
            Assert.Equal(2, registrations.Count);
            Assert.Equal("2025-01", registrations[0].Month);
        }

        [Fact]
        public async Task GetSpaceStats_SuperAdmin_ReturnsOk()
        {
            // Arrange
            SetupUser("SuperAdmin", "superadmin-1");
            var stats = new SpaceStats
            {
                TotalSpaces = 50,
                SpacesByProvider = new List<ProviderSpace>
                {
                    new() { ProviderId = 1, Spaces = 20 },
                    new() { ProviderId = 2, Spaces = 30 }
                }
            };
            _statsServiceMock.Setup(s => s.GetSpaceStats()).ReturnsAsync(stats);

            // Act
            var result = await _controller.GetSpaceStats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(okResult.Value));
            Assert.Equal(50, Convert.ToInt32(response["totalSpaces"]));
            var byProvider = JsonSerializer.Deserialize<List<ProviderSpace>>(response["byProvider"].ToString());
            Assert.Equal(20, byProvider[0].Spaces);
        }

        #endregion

        #region Admin Tests

        [Fact]
        public async Task GetAdminUserStats_Admin_ReturnsOk()
        {
            // Arrange
            SetupUser("Admin", "admin-1");
            var stats = new AdminUserStats { TotalUsers = 50 };
            _statsServiceMock.Setup(s => s.GetAdminUserStats()).ReturnsAsync(stats);

            // Act
            var result = await _controller.GetAdminUserStats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(okResult.Value));
            Assert.Equal(50, Convert.ToInt32(response["totalUsers"]));
        }

        [Fact]
        public async Task GetAdminBookingStats_Admin_ReturnsOk()
        {
            // Arrange
            SetupUser("Admin", "admin-1");
            var stats = new AdminBookingStats
            {
                TotalBookings = 100,
                BookingsByMonth = new List<MonthlyBookings>
                {
                    new() { Month = "2025-01", Count = 40 },
                    new() { Month = "2025-02", Count = 60 }
                }
            };
            _statsServiceMock.Setup(s => s.GetAdminBookingStats()).ReturnsAsync(stats);

            // Act
            var result = await _controller.GetAdminBookingStats();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(okResult.Value));
            Assert.Equal(100, Convert.ToInt32(response["totalBookings"]));
            var byMonth = JsonSerializer.Deserialize<List<MonthlyBookings>>(response["byMonth"].ToString());
            Assert.Equal(40, byMonth[0].Count);
        }

        #endregion

        #region Provider Tests

        [Fact]
        public async Task GetProviderSpaceStats_Provider_Authorized_ReturnsOk()
        {
            // Arrange
            SetupUser("Provider", "provider-user-1");
            var providerId = 1;
            var stats = new ProviderSpaceStats { TotalSpaces = 10 };
            _statsServiceMock.Setup(s => s.IsProviderAuthorized("provider-user-1", providerId)).ReturnsAsync(true);
            _statsServiceMock.Setup(s => s.GetProviderSpaceStats(providerId)).ReturnsAsync(stats);

            // Act
            var result = await _controller.GetProviderSpaceStats(providerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(okResult.Value));
            Assert.Equal(10, Convert.ToInt32(response["totalSpaces"]));
        }

        [Fact]
        public async Task GetProviderSpaceStats_Provider_Unauthorized_Returns401()
        {
            // Arrange
            SetupUser("Provider", "provider-user-1");
            var providerId = 1;
            _statsServiceMock.Setup(s => s.IsProviderAuthorized("provider-user-1", providerId)).ReturnsAsync(false);

            // Act
            var result = await _controller.GetProviderSpaceStats(providerId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task GetProviderBookingStats_Provider_Authorized_ReturnsOk()
        {
            // Arrange
            SetupUser("Provider", "provider-user-1");
            var providerId = 1;
            var stats = new ProviderBookingStats
            {
                TotalBookings = 30,
                BookingsByDay = new List<DailyBookings>
                {
                    new() { Date = "2025-05-01", Count = 5 },
                    new() { Date = "2025-05-02", Count = 10 }
                }
            };
            _statsServiceMock.Setup(s => s.IsProviderAuthorized("provider-user-1", providerId)).ReturnsAsync(true);
            _statsServiceMock.Setup(s => s.GetProviderBookingStats(providerId)).ReturnsAsync(stats);

            // Act
            var result = await _controller.GetProviderBookingStats(providerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(okResult.Value));
            Assert.Equal(30, Convert.ToInt32(response["totalBookings"]));
            var byDay = JsonSerializer.Deserialize<List<DailyBookings>>(response["byDay"].ToString());
            Assert.Equal(5, byDay[0].Count);
        }

        [Fact]
        public async Task GetProviderOccupancyStats_Provider_Authorized_ReturnsOk()
        {
            // Arrange
            SetupUser("Provider", "provider-user-1");
            var providerId = 1;
            var stats = new ProviderOccupancyStats
            {
                OccupancyRate = 0.75,
                OccupancyByDay = new List<DailyOccupancy>
                {
                    new() { Date = "2025-05-01", Rate = 0.7 },
                    new() { Date = "2025-05-02", Rate = 0.8 }
                }
            };
            _statsServiceMock.Setup(s => s.IsProviderAuthorized("provider-user-1", providerId)).ReturnsAsync(true);
            _statsServiceMock.Setup(s => s.GetProviderOccupancyStats(providerId)).ReturnsAsync(stats);

            // Act
            var result = await _controller.GetProviderOccupancyStats(providerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(okResult.Value));
            Assert.Equal(0.75, Convert.ToDouble(response["occupancyRate"]));
            var byDay = JsonSerializer.Deserialize<List<DailyOccupancy>>(response["byDay"].ToString());
            Assert.Equal(0.7, byDay[0].Rate);
        }

        #endregion

        #region Client Tests

        [Fact]
        public async Task GetClientBookingStats_Client_Authorized_ReturnsOk()
        {
            // Arrange
            SetupUser("Client", "client-1");
            var userId = "client-1";
            var stats = new ClientBookingStats
            {
                TotalBookings = 5,
                Bookings = new List<BookingDetail>
                {
                    new() { SpaceId = "space-1", Date = "2025-05-01" },
                    new() { SpaceId = "space-2", Date = "2025-05-02" }
                }
            };
            _statsServiceMock.Setup(s => s.GetClientBookingStats(userId)).ReturnsAsync(stats);

            // Act
            var result = await _controller.GetClientBookingStats(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(okResult.Value));
            Assert.Equal(5, Convert.ToInt32(response["totalBookings"]));
            var bookings = JsonSerializer.Deserialize<List<BookingDetail>>(response["bookings"].ToString());
            Assert.Equal("space-1", bookings[0].SpaceId);
        }

        [Fact]
        public async Task GetClientBookingStats_Client_Unauthorized_Returns401()
        {
            // Arrange
            SetupUser("Client", "client-1");
            var userId = "client-2"; // Diferente userId

            // Act
            var result = await _controller.GetClientBookingStats(userId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task GetClientActivityStats_Client_Authorized_ReturnsOk()
        {
            // Arrange
            SetupUser("Client", "client-1");
            var userId = "client-1";
            var stats = new ClientActivityStats
            {
                TotalActions = 10,
                Actions = new List<ActionDetail>
                {
                    new() { Date = "2025-05-01", ActionType = "Login" },
                    new() { Date = "2025-05-02", ActionType = "Search" }
                }
            };
            _statsServiceMock.Setup(s => s.GetClientActivityStats(userId)).ReturnsAsync(stats);

            // Act
            var result = await _controller.GetClientActivityStats(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(okResult.Value));
            Assert.Equal(10, Convert.ToInt32(response["totalActions"]));
            var actions = JsonSerializer.Deserialize<List<ActionDetail>>(response["actions"].ToString());
            Assert.Equal("Login", actions[0].ActionType);
        }

        [Fact]
        public async Task GetClientActivityStats_Client_Unauthorized_Returns401()
        {
            // Arrange
            SetupUser("Client", "client-1");
            var userId = "client-2"; // Diferente userId

            // Act
            var result = await _controller.GetClientActivityStats(userId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        #endregion
    }
}