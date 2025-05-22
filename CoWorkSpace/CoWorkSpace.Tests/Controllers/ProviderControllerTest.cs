using CoWorkSpace.Api.Controllers;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CoWorkSpace.Tests
{
    public class ProviderControllerTests
    {
        private CoWorkSpaceContext GetDbContextWithData()
        {
            var options = new DbContextOptionsBuilder<CoWorkSpaceContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new CoWorkSpaceContext(options);

            context.Roles.AddRange(
                new Role { RoleId = 2, RoleName = "Admin" },
                new Role { RoleId = 3, RoleName = "Provider" }
            );

            context.Users.Add(new User
            {
                Id = 1,
                Email = "provider@coworkspace.com",
                Name = "Test Provider",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Provider123"),
                RoleId = 3,
                ProviderId = null
            });

            context.SaveChanges();

            return context;
        }

        private string GetMessageFromResult(IActionResult result)
        {
            var objectResult = result as ObjectResult;
            if (objectResult?.Value == null)
                return null;

            var messageProperty = objectResult.Value.GetType().GetProperty("message");
            return messageProperty?.GetValue(objectResult.Value)?.ToString();
        }

        [Fact]
        public async Task CreateAdmin_Returns_Ok_If_Valid()
        {
            // Arrange
            var context = GetDbContextWithData();
            var providerId = 1; // ID del Provider existente
            var claims = new List<Claim>
            {
                new Claim("roleId", "3"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            var controller = new ProviderController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            var dto = new RegisterRequestDTO
            {
                Email = "admin@coworkspace.com",
                Password = "Admin123",
                Name = "New Admin",
                RoleId = 2
            };

            // Act
            var result = await controller.CreateAdmin(providerId, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = GetMessageFromResult(okResult);
            Assert.Equal(ApiMessages.AdminCreatedSuccessfully, message);

            var admin = await context.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
            Assert.NotNull(admin);
            Assert.Equal(2, admin.RoleId);
            Assert.Equal(providerId, admin.ProviderId);
            Assert.True(BCrypt.Net.BCrypt.Verify(dto.Password, admin.PasswordHash));
        }

        [Fact]
        public async Task CreateAdmin_Returns_BadRequest_If_ModelInvalid()
        {
            // Arrange
            var context = GetDbContextWithData();
            var providerId = 1;
            var claims = new List<Claim>
            {
                new Claim("roleId", "3"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            var controller = new ProviderController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
            controller.ModelState.AddModelError("Email", ApiMessages.MailRequired);

            var dto = new RegisterRequestDTO
            {
                Email = null, // Inválido
                Password = "Admin123",
                Name = "New Admin",
                RoleId = 2
            };

            // Act
            var result = await controller.CreateAdmin(providerId, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateAdmin_Returns_Unauthorized_If_NoClaims()
        {
            // Arrange
            var context = GetDbContextWithData();
            var providerId = 1;
            var controller = new ProviderController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
                }
            };

            var dto = new RegisterRequestDTO
            {
                Email = "admin@coworkspace.com",
                Password = "Admin123",
                Name = "New Admin",
                RoleId = 2
            };

            // Act
            var result = await controller.CreateAdmin(providerId, dto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var message = GetMessageFromResult(unauthorizedResult);
            Assert.Equal(ApiMessages.Unauthorized, message);
        }

        [Fact]
        public async Task CreateAdmin_Returns_Unauthorized_If_NotProvider()
        {
            // Arrange
            var context = GetDbContextWithData();
            var providerId = 1;
            var claims = new List<Claim>
            {
                new Claim("roleId", "4"), // Client, no Provider
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            var controller = new ProviderController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            var dto = new RegisterRequestDTO
            {
                Email = "admin@coworkspace.com",
                Password = "Admin123",
                Name = "New Admin",
                RoleId = 2
            };

            // Act
            var result = await controller.CreateAdmin(providerId, dto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var message = GetMessageFromResult(unauthorizedResult);
            Assert.Equal(ApiMessages.OnlyProvidersCanCreateAdmins, message);
        }

        [Fact]
        public async Task CreateAdmin_Returns_Unauthorized_If_WrongProviderId()
        {
            // Arrange
            var context = GetDbContextWithData();
            var providerId = 2; // Diferente al userId
            var claims = new List<Claim>
            {
                new Claim("roleId", "3"),
                new Claim(ClaimTypes.NameIdentifier, "1") // userId = 1
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            var controller = new ProviderController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            var dto = new RegisterRequestDTO
            {
                Email = "admin@coworkspace.com",
                Password = "Admin123",
                Name = "New Admin",
                RoleId = 2
            };

            // Act
            var result = await controller.CreateAdmin(providerId, dto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var message = GetMessageFromResult(unauthorizedResult);
            Assert.Equal(ApiMessages.CannotCreateAdminsForOtherProviders, message);
        }

        [Fact]
        public async Task CreateAdmin_Returns_BadRequest_If_RoleNotAdmin()
        {
            // Arrange
            var context = GetDbContextWithData();
            var providerId = 1;
            var claims = new List<Claim>
            {
                new Claim("roleId", "3"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            var controller = new ProviderController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            var dto = new RegisterRequestDTO
            {
                Email = "admin@coworkspace.com",
                Password = "Admin123",
                Name = "New Admin",
                RoleId = 4 // Client, no Admin
            };

            // Act
            var result = await controller.CreateAdmin(providerId, dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var message = GetMessageFromResult(badRequest);
            Assert.Equal(ApiMessages.OnlyAdminRoleAllowed, message);
        }

        [Fact]
        public async Task CreateAdmin_Returns_BadRequest_If_EmailExists()
        {
            // Arrange
            var context = GetDbContextWithData();
            var providerId = 1;
            var claims = new List<Claim>
            {
                new Claim("roleId", "3"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            var controller = new ProviderController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            var dto = new RegisterRequestDTO
            {
                Email = "provider@coworkspace.com", // Email ya registrado
                Password = "Admin123",
                Name = "New Admin",
                RoleId = 2
            };

            // Act
            var result = await controller.CreateAdmin(providerId, dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var message = GetMessageFromResult(badRequest);
            Assert.Equal(ApiMessages.EmailAlreadyRegistered, message);
        }
    }
}