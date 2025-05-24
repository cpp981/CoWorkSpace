using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoWorkSpace.Api.Controllers;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoWorkSpace.Api.Tests
{
    public class RoleControllerTests
    {
        private readonly Mock<CoWorkSpaceContext> _mockContext;
        private readonly RoleController _controller;

        public RoleControllerTests()
        {
            // Inicializar el Mock de CoWorkSpaceContext sin llamar al constructor real
            _mockContext = new Mock<CoWorkSpaceContext>(MockBehavior.Strict);
            _controller = new RoleController(_mockContext.Object);
        }

        [Fact]
        public async Task GetRoles_ReturnsOkResult_WithFilteredRoles()
        {
            // Arrange
            var roles = new List<Role>
            {
                new Role { RoleId = 1, RoleName = "SuperAdmin" },
                new Role { RoleId = 2, RoleName = "Admin" },
                new Role { RoleId = 3, RoleName = "Provider" },
                new Role { RoleId = 4, RoleName = "Client" }
            };

            var mockSet = CreateMockDbSet(roles);
            _mockContext.Setup(c => c.Roles).Returns(mockSet.Object);

            // Act
            var result = await _controller.GetRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<object>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);

            var returnedRoles = returnValue.Cast<dynamic>().ToList();
            Assert.Equal(3, returnedRoles[0].Id);
            Assert.Equal("Proveedor", returnedRoles[0].Name);
            Assert.Equal(4, returnedRoles[1].Id);
            Assert.Equal("Cliente", returnedRoles[1].Name);
        }

        [Fact]
        public async Task GetRoles_ReturnsOkResult_WithEmptyList_WhenNoValidRoles()
        {
            // Arrange
            var roles = new List<Role>
            {
                new Role { RoleId = 1, RoleName = "SuperAdmin" },
                new Role { RoleId = 2, RoleName = "Admin" }
            };

            var mockSet = CreateMockDbSet(roles);
            _mockContext.Setup(c => c.Roles).Returns(mockSet.Object);

            // Act
            var result = await _controller.GetRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<object>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task GetRoles_ReturnsOkResult_WithEmptyList_WhenDatabaseEmpty()
        {
            // Arrange
            var roles = new List<Role>();

            var mockSet = CreateMockDbSet(roles);
            _mockContext.Setup(c => c.Roles).Returns(mockSet.Object);

            // Act
            var result = await _controller.GetRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<object>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        private static Mock<DbSet<T>> CreateMockDbSet<T>(IList<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            return mockSet;
        }
    }
}