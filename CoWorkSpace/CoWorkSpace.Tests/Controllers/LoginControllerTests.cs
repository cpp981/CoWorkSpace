using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using CoWorkSpace.Api.Controllers;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Models;

namespace CoWorkSpace.Tests
{
    public class LoginControllerTests
    {
        private CoWorkSpaceContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<CoWorkSpaceContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // base limpia para cada test
                .Options;

            var context = new CoWorkSpaceContext(options);

            // Añadimos un usuario con hash para "12345678"
            var user = new User
            {
                Id = 2,
                Email = "provider@coworkspace.com",
                Name = "Test Provider",
                PasswordHash = "$2a$11$ix99XlIasCCcYr/Zz5AwzO5TTr.Zv.ykHWwRULTo4NyWTSr9J3W5W", // hash "12345678"
                RoleId = 3,
                Role = new Role { RoleId = 3, RoleName = "Provider" }
            };

            context.Users.Add(user);
            context.SaveChanges();

            return context;
        }

        private IConfiguration GetTestConfiguration()
        {
            var inMemorySettings = new System.Collections.Generic.Dictionary<string, string> {
                {"Jwt:Key", "EstaEsUnaClaveSuperSecreta123456789!"},
                {"Jwt:Issuer", "coworkspace"},
                {"Jwt:Audience", "coworkspaceUsers"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task Login_Should_ReturnToken_When_CredentialsAreValid()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var configuration = GetTestConfiguration();
            var controller = new LoginController(context, configuration);

            var loginDto = new LoginRequestDto
            {
                Email = "provider@coworkspace.com",
                Password = "Provider123"
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponseDto>(okResult.Value);
            Assert.False(string.IsNullOrEmpty(response.Token));
        }

        [Fact]
        public async Task Login_Should_ReturnUnauthorized_When_PasswordIsInvalid()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var configuration = GetTestConfiguration();
            var controller = new LoginController(context, configuration);

            var loginDto = new LoginRequestDto
            {
                Email = "provider@coworkspace.com",
                Password = "wrongpassword"
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Credenciales inválidas.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_Should_ReturnUnauthorized_When_EmailNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var configuration = GetTestConfiguration();
            var controller = new LoginController(context, configuration);

            var loginDto = new LoginRequestDto
            {
                Email = "noexiste@coworkspace.com",
                Password = "12345678"
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Credenciales inválidas.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_Should_ReturnBadRequest_When_LoginDtoIsNullOrEmpty()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var configuration = GetTestConfiguration();
            var controller = new LoginController(context, configuration);

            // Act
            var resultNull = await controller.Login(null);
            var resultEmpty = await controller.Login(new LoginRequestDto { Email = "", Password = "" });

            // Assert
            var badRequestNull = Assert.IsType<BadRequestObjectResult>(resultNull);
            var badRequestEmpty = Assert.IsType<BadRequestObjectResult>(resultEmpty);

            Assert.Equal("Email y contraseña son requeridos.", badRequestNull.Value);
            Assert.Equal("Email y contraseña son requeridos.", badRequestEmpty.Value);
        }
    }
}

