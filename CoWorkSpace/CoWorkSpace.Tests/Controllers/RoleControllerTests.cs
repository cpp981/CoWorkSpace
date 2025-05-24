using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoWorkSpace.Api.Controllers;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoWorkSpace.Api.Tests
{
    public class RoleControllerTests
    {
        private CoWorkSpaceContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<CoWorkSpaceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new CoWorkSpaceContext(options);
        }

        [Fact]
        public async Task GetRoles_ReturnsOkResult_WithFilteredRoles()
        {
            // Arrange
            using var context = GetInMemoryContext();
            context.Roles.AddRange(new List<Role>
            {
                new Role { RoleId = 1, RoleName = "SuperAdmin" },
                new Role { RoleId = 2, RoleName = "Admin" },
                new Role { RoleId = 3, RoleName = "Provider" },
                new Role { RoleId = 4, RoleName = "Client" }
            });
            await context.SaveChangesAsync();
            var controller = new RoleController(context);

            // Act
            var result = await controller.GetRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value as IEnumerable<object>;
            Assert.NotNull(returnValue);
            var returnedRoles = returnValue.ToList();
            Assert.Equal(2, returnedRoles.Count);

            // Comparar con tipo anónimo esperado
            var expectedRoles = new[]
            {
                new { Id = 3, Name = "Provider" },
                new { Id = 4, Name = "Client" }
            }.ToList();

            for (int i = 0; i < returnedRoles.Count; i++)
            {
                var actual = returnedRoles[i];
                var expected = expectedRoles[i];
                Assert.Equal(expected.Id, actual.GetType().GetProperty("Id").GetValue(actual));
                Assert.Equal(expected.Name, actual.GetType().GetProperty("Name").GetValue(actual));
            }
        }

        [Fact]
        public async Task GetRoles_ReturnsOkResult_WithEmptyList_WhenNoValidRoles()
        {
            // Arrange
            using var context = GetInMemoryContext();
            context.Roles.AddRange(new List<Role>
            {
                new Role { RoleId = 1, RoleName = "SuperAdmin" },
                new Role { RoleId = 2, RoleName = "Admin" }
            });
            await context.SaveChangesAsync();
            var controller = new RoleController(context);

            // Act
            var result = await controller.GetRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value as IEnumerable<object>;
            Assert.NotNull(returnValue);
            var returnedRoles = returnValue.ToList();
            Assert.Empty(returnedRoles);
        }

        [Fact]
        public async Task GetRoles_ReturnsOkResult_WithEmptyList_WhenDatabaseEmpty()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var controller = new RoleController(context);

            // Act
            var result = await controller.GetRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value as IEnumerable<object>;
            Assert.NotNull(returnValue);
            var returnedRoles = returnValue.ToList();
            Assert.Empty(returnedRoles);
        }
    }
}