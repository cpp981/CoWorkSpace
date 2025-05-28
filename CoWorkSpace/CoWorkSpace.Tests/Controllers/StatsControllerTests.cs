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
            var options = new DbContextOptionsBuilder<CoWorkSpaceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            var context = new CoWorkSpaceContext(options);

            // Roles
            context.Roles.Add(new Role { RoleId = 1, RoleName = "SuperAdmin" });
            context.Roles.Add(new Role { RoleId = 2, RoleName = "Admin" });
            context.Roles.Add(new Role { RoleId = 3, RoleName = "Provider" });
            context.Roles.Add(new Role { RoleId = 4, RoleName = "Client" });

            // Users
            context.Users.Add(new User
            {
                Id = 1,
                Email = "superadmin@coworkspace.com",
                Name = "Super Admin",
                RoleId = 1,
                PasswordHash = "$2a$11$LSiUCLVeVeOynzKQVsM2XOq4jS3IBhsJ.VzouEqCmjQjhty4l.3Pa",
                ProviderId = null
            });
            context.Users.Add(new User
            {
                Id = 2,
                Email = "provider@coworkspace.com",
                Name = "Test Provider",
                RoleId = 3,
                PasswordHash = "$2a$11$ix99XlIasCCcYr/Zz5AwzO5TTr.Zv.ykHWwRULTo4NyWTSr9J3W5W",
                ProviderId = null
            });
            context.Users.Add(new User
            {
                Id = 3,
                Email = "admin@coworkspace.com",
                Name = "Test Admin",
                RoleId = 2,
                ProviderId = 2,
                PasswordHash = "$2a$11$Kn0nDdok.EqeppL6E0rTje40JdNq0qP8Pz.D/YtISJBgH1UgRrvqG"
            });
            context.Users.Add(new User
            {
                Id = 4,
                Email = "client@coworkspace.com",
                Name = "Test Client",
                RoleId = 4,
                PasswordHash = "$2a$11$R6e5nDM1HoXKHFhxALf4B.jQpJ7tko/p5zY.R.e7QCloUrOEMtoRe",
                ProviderId = null
            });

            // Spaces
            context.Spaces.Add(new Space
            {
                Id = 1,
                Name = "Test Space",
                AdminId = 3,
                ProviderId = 2,
                IsPublic = true,
                Price = 50.00m,
                City = "Madrid",
                IsDeleted = false
            });

            // Bookings
            context.Bookings.Add(new Booking
            {
                Id = 1,
                UserId = 4,
                SpaceId = 1,
                StartTime = new DateTime(2025, 5, 21, 10, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2025, 5, 21, 12, 0, 0, DateTimeKind.Utc)
            });

            // Payments
            context.Payments.Add(new Payment
            {
                Id = 1,
                BookingId = 1,
                SpaceId = null,
                UserId = 4,
                Amount = 100.00m,
                Status = "CREADO"
            });
            context.Payments.Add(new Payment
            {
                Id = 2,
                BookingId = null,
                SpaceId = 1,
                UserId = 3,
                Amount = 50.00m,
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

            // Logs
            context.Logs.Add(new Log
            {
                Id = 1,
                Timestamp = new DateTime(2025, 5, 19, 23, 59, 59, DateTimeKind.Utc),
                EventType = "SystemStartup",
                UserId = null,
                Details = "Sistema iniciado correctamente."
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
            var actionResult = Assert.IsType<ActionResult<SuperAdminStatsDTO>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
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
            var actionResult = Assert.IsType<ActionResult<SuperAdminStatsDTO>>(result);
            Assert.IsType<ForbidResult>(actionResult.Result);
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
            var actionResult = Assert.IsType<ActionResult<SuperAdminStatsDTO>>(result);
            Assert.IsType<ForbidResult>(actionResult.Result);
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
            var actionResult = Assert.IsType<ActionResult<AdminStatsDTO>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
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
            var actionResult = Assert.IsType<ActionResult<AdminStatsDTO>>(result);
            Assert.IsType<ForbidResult>(actionResult.Result);
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
            var actionResult = Assert.IsType<ActionResult<AdminStatsDTO>>(result);
            Assert.IsType<ForbidResult>(actionResult.Result);
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
            var actionResult = Assert.IsType<ActionResult<ProviderStatsDTO>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
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
            var actionResult = Assert.IsType<ActionResult<ProviderStatsDTO>>(result);
            Assert.IsType<ForbidResult>(actionResult.Result);
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
            var actionResult = Assert.IsType<ActionResult<ProviderStatsDTO>>(result);
            Assert.IsType<ForbidResult>(actionResult.Result);
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
            var actionResult = Assert.IsType<ActionResult<ClientStatsDTO>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
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
            var actionResult = Assert.IsType<ActionResult<ClientStatsDTO>>(result);
            Assert.IsType<ForbidResult>(actionResult.Result);
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
            var actionResult = Assert.IsType<ActionResult<ClientStatsDTO>>(result);
            Assert.IsType<ForbidResult>(actionResult.Result);
        }
    }
}