using CoWorkSpace.Api.Controllers;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Models;
using CoWorkSpace.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace CoWorkSpace.Api.Tests.Controllers
{
    public class StatsControllerTests
    {
        private CoWorkSpaceContext GetDbContextWithData()
        {
            var options = new DbContextOptionsBuilder<CoWorkSpaceContext>().UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            var context = new CoWorkSpaceContext(options);

            // Roles
            context.Roles.AddRange(
                new Role { RoleId = 1, RoleName = "SuperAdmin" },
                new Role { RoleId = 2, RoleName = "Admin" },
                new Role { RoleId = 3, RoleName = "Provider" },
                new Role { RoleId = 4, RoleName = "Client" }
            );

            // Users
            context.Users.AddRange(
                new User { Id = 1, Email = "superadmin@coworkspace.com", Name = "Super Admin", RoleId = 1, PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin123") },
                new User { Id = 2, Email = "provider@coworkspace.com", Name = "Provider", RoleId = 3, PasswordHash = BCrypt.Net.BCrypt.HashPassword("Provider123") },
                new User { Id = 3, Email = "admin@coworkspace.com", Name = "Admin", RoleId = 2, ProviderId = 2, PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123") },
                new User { Id = 4, Email = "client@coworkspace.com", Name = "Client", RoleId = 4, PasswordHash = BCrypt.Net.BCrypt.HashPassword("Client123") }
             );

            // Spaces
            context.Spaces.Add(new Space
            {
                Id = 1,
                Name = "Test Space",
                City = "Madrid",
                AdminId = 3,
                ProviderId = 2,
                IsDeleted = false
            });

            // Bookings
            context.Bookings.Add(new Booking
            {
                Id = 1,
                UserId = 4,
                SpaceId = 1,
                StartTime = DateTime.UtcNow
            });

            // Payments
            context.Payments.Add(new Payment
            {
                Id = 1,
                BookingId = 1,
                SpaceId = 1,
                UserId = 4,
                Amount = 100m,
                Status = "CREADO"
            });

            // Reviews
            context.Reviews.Add(new Review
            {
                Id = 1,
                UserId = 4,
                SpaceId = 1,
                Rating = 5,
                Comment = "Excelente espacio de trabajo!"
            });

            context.SaveChanges();
            return context;
        }

        private void SetupClaims(ControllerBase controller, int userId, int roleId)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("roleId", roleId.ToString())
        };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task GetSuperAdminStats_ValidUserAndRole_ReturnsOkWithStats()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new StatsController(context);
            SetupClaims(controller, 1, 1);

            // Act
            var result = await controller.GetSuperAdminStats(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var stats = Assert.IsType<SuperAdminStatsDTO>(okResult.Value);
            Assert.Equal(1, stats.TotalSpaces);
            Assert.Equal(1, stats.TotalBookings);
            Assert.Equal(100m, stats.TotalRevenue);
            Assert.Equal(4, stats.TotalUsers);
            Assert.Equal(4, stats.UsersByRole.Count);
            Assert.Equal(1, stats.UsersByRole["SuperAdmin"]);
            Assert.Equal(1, stats.UsersByRole["Admin"]);
            Assert.Equal(1, stats.UsersByRole["Provider"]);
            Assert.Equal(1, stats.UsersByRole["Client"]);
        }

        [Fact]
        public async Task GetSuperAdminStats_WrongUserId_ReturnsForbid()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new StatsController(context);
            SetupClaims(controller, 2, 1);

            // Act
            var result = await controller.GetSuperAdminStats(1);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetSuperAdminStats_WrongRoleId_ReturnsForbid()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new StatsController(context);
            SetupClaims(controller, 1, 2);

            // Act
            var result = await controller.GetSuperAdminStats(1);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetAdminStats_ValidUserAndRole_ReturnsOkWithStats()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new StatsController(context);
            SetupClaims(controller, 3, 2);

            // Act
            var result = await controller.GetAdminStats(3);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var stats = Assert.IsType<AdminStatsDTO>(okResult.Value);
            Assert.Equal(1, stats.TotalSpaces);
            Assert.Equal(1, stats.TotalBookings);
            Assert.Equal(100m, stats.TotalRevenue);
            Assert.Equal(5.0, stats.AverageRating);
            Assert.Single(stats.Spaces);
            var space = stats.Spaces[0];
            Assert.Equal(1, space.SpaceId);
            Assert.Equal("Test Space", space.SpaceName);
            Assert.Equal(1, space.BookingsCount);
            Assert.Equal(100m, space.Revenue);
            Assert.Equal(5.0, space.AverageRating);
        }

        [Fact]
        public async Task GetAdminStats_WrongUserId_ReturnsForbid()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new StatsController(context);
            SetupClaims(controller, 4, 2);

            // Act
            var result = await controller.GetAdminStats(3);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetAdminStats_WrongRoleId_ReturnsForbid()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new StatsController(context);
            SetupClaims(controller, 3, 3);

            // Act
            var result = await controller.GetAdminStats(3);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetProviderStats_ValidUserAndRole_ReturnsOkWithStats()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new StatsController(context);
            SetupClaims(controller, 2, 3);

            // Act
            var result = await controller.GetProviderStats(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var stats = Assert.IsType<ProviderStatsDTO>(okResult.Value);
            Assert.Equal(1, stats.TotalSpaces);
            Assert.Equal(1, stats.TotalAdmins);
            Assert.Equal(1, stats.TotalBookings);
            Assert.Equal(100m, stats.TotalRevenue);
            Assert.Single(stats.Spaces);
            var space = stats.Spaces[0];
            Assert.Equal(1, space.SpaceId);
            Assert.Equal("Test Space", space.SpaceName);
            Assert.Equal(3, space.AdminId);
            Assert.Equal("Admin", space.AdminName);
            Assert.Equal(1, space.BookingsCount);
            Assert.Equal(100m, space.Revenue);
        }

        [Fact]
        public async Task GetProviderStats_WrongUserId_ReturnsForbid()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new StatsController(context);
            SetupClaims(controller, 1, 3);

            // Act
            var result = await controller.GetProviderStats(2);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetProviderStats_WrongRoleId_ReturnsForbid()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new StatsController(context);
            SetupClaims(controller, 2, 4);

            // Act
            var result = await controller.GetProviderStats(2);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetClientStats_ValidUserAndRole_ReturnsOkWithStats()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new StatsController(context);
            SetupClaims(controller, 4, 4);

            // Act
            var result = await controller.GetClientStats(4);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var stats = Assert.IsType<ClientStatsDTO>(okResult.Value);
            Assert.Equal(1, stats.TotalBookings);
            Assert.Equal(100m, stats.TotalSpent);
            Assert.Equal(1, stats.TotalReviews);
            Assert.Single(stats.Bookings);
            var booking = stats.Bookings[0];
            Assert.Equal(1, booking.BookingId);
            Assert.Equal(1, booking.SpaceId);
            Assert.Equal("Test Space", booking.Name);
            Assert.Equal(100m, booking.Amount);
        }

        [Fact]
        public async Task GetClientStats_WrongUserId_ReturnsForbid()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new StatsController(context);
            SetupClaims(controller, 5, 4);

            // Act
            var result = await controller.GetClientStats(4);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetClientStats_WrongRoleId_ReturnsForbid()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new StatsController(context);
            SetupClaims(controller, 4, 3);

            // Act
            var result = await controller.GetClientStats(4);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }
}