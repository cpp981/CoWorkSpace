using Xunit;
using Microsoft.AspNetCore.Mvc;
using CoWorkSpace.Api.Controllers;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Models;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.Constants;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace CoWorkSpace.Tests
{
    public class AuthControllerTests
    {
        private CoWorkSpaceContext GetDbContextWithData()
        {
            var options = new DbContextOptionsBuilder<CoWorkSpaceContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new CoWorkSpaceContext(options);

            context.Roles.AddRange(
                new Role { RoleId = 3, RoleName = "Provider" },
                new Role { RoleId = 4, RoleName = "Client" }
            );

            context.Users.Add(new User
            {
                Email = "provider@coworkspace.com",
                Name = "Test Provider",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Provider123"),
                RoleId = 3
            });

            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task Register_Returns_BadRequest_If_Email_Exists()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new AuthController(context);

            var dto = new RegisterRequestDTO
            {
                Email = "provider@coworkspace.com",
                Password = "Test123",
                Name = "Test",
                RoleId = 3
            };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<RegisterResponseDTO>(badRequest.Value);
            Assert.Equal(ApiMessages.EMAIL_ALREADY_REGISTERED, response.Message);
        }

        [Fact]
        public async Task Register_Returns_BadRequest_If_RoleId_Not_Allowed()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new AuthController(context);

            var dto = new RegisterRequestDTO
            {
                Email = "new@coworkspace.com",
                Password = "Test123",
                Name = "Test",
                RoleId = 2
            };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<RegisterResponseDTO>(badRequest.Value);
            Assert.Equal(ApiMessages.ROLE_NOT_ALLOWED_ONLY_CAN_REGISTER_PROVIDER_OR_CLIENT, response.Message);
        }

        [Fact]
        public async Task Register_Returns_BadRequest_If_RoleId_Not_Exists()
        {
            // Arrange
            var context = new CoWorkSpaceContext(new DbContextOptionsBuilder<CoWorkSpaceContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            context.Roles.Add(new Role { RoleId = 3, RoleName = "Provider" });
            await context.SaveChangesAsync();

            var controller = new AuthController(context);

            var dto = new RegisterRequestDTO
            {
                Email = "test@cowork.com",
                Password = "Test123!",
                Name = "Test User",
                RoleId = 4
            };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<RegisterResponseDTO>(badRequest.Value);
            Assert.Equal(ApiMessages.INVALID_ROLE_OR_ROLEID_NOT_FOUND, response.Message);
        }

        [Fact]
        public async Task Register_Returns_Ok_If_Valid()
        {
            // Arrange
            var context = GetDbContextWithData();
            var controller = new AuthController(context);

            var dto = new RegisterRequestDTO
            {
                Email = "valid@coworkspace.com",
                Password = "Valid123",
                Name = "New User",
                RoleId = 3
            };

            // Act
            var result = await controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<RegisterResponseDTO>(okResult.Value);
            Assert.Equal(ApiMessages.USER_REGISTERED_SUCCESS, response.Message);
            Assert.Equal(dto.Email, response.Email);
            Assert.Equal(dto.Name, response.Name);
            Assert.Equal(dto.RoleId, response.RoleId);
            Assert.Null(response.ProviderId);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            Assert.NotNull(user);
            Assert.True(BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash));
        }
    }
}
