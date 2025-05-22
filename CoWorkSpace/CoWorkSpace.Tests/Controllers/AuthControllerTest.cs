using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using CoWorkSpace.Api.Controllers;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Models;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.Constants;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
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

        private string GetMessageFromResult(IActionResult result)
        {
            var objectResult = result as ObjectResult;
            if (objectResult?.Value == null)
                return null;

            var messageProperty = objectResult.Value.GetType().GetProperty("message");
            return messageProperty?.GetValue(objectResult.Value)?.ToString();
        }

        [Fact]
        public async Task Register_Returns_BadRequest_If_Email_Exists()
        {
            var context = GetDbContextWithData();
            var controller = new AuthController(context);

            var dto = new RegisterRequestDTO
            {
                Email = "provider@coworkspace.com",
                Password = "Test123",
                Name = "Test",
                RoleId = 3
            };

            var result = await controller.Register(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var message = GetMessageFromResult(badRequest);
            Assert.Equal(ApiMessages.EmailAlreadyRegistered, message);
        }

        [Fact]
        public async Task Register_Returns_BadRequest_If_RoleId_Not_Allowed()
        {
            var context = GetDbContextWithData();
            var controller = new AuthController(context);

            var dto = new RegisterRequestDTO
            {
                Email = "new@coworkspace.com",
                Password = "Test123",
                Name = "Test",
                RoleId = 2
            };

            var result = await controller.Register(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var message = GetMessageFromResult(badRequest);
            Assert.Equal(ApiMessages.RoleNotAllowedOnlyCanRegisterProviderOrClient, message);
        }

        [Fact]
        public async Task Register_Returns_BadRequest_If_RoleId_Not_Exists()
        {
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

            var result = await controller.Register(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var message = GetMessageFromResult(result);
            Assert.Equal(ApiMessages.InvalidRoleOrRoleIdNotFound, message);
        }

        [Fact]
        public async Task Register_Returns_Ok_If_Valid()
        {
            var context = GetDbContextWithData();
            var controller = new AuthController(context);

            var dto = new RegisterRequestDTO
            {
                Email = "valid@coworkspace.com",
                Password = "Valid123",
                Name = "New User",
                RoleId = 3
            };

            var result = await controller.Register(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = GetMessageFromResult(okResult);
            Assert.Equal(ApiMessages.UserRegisteredSuccessfully, message);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            Assert.NotNull(user);
            Assert.True(BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash));
        }
    }
}
