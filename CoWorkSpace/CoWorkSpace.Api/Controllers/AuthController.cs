using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.Models;
using CoWorkSpace.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly CoWorkSpaceContext _context;

        public AuthController(CoWorkSpaceContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        [Authorize]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Obtener el claim "roleId"
            var roleIdClaim = User.FindFirst("roleId")?.Value;
            if (roleIdClaim == null || !int.TryParse(roleIdClaim, out int userRoleId))
                return Unauthorized("No autorizado. Rol inválido o no encontrado.");

            // Permitir solo roles 3 y 4 para registrar
            if (userRoleId != 3 && userRoleId != 4)
                return Unauthorized(new { message = "No tiene permisos para registrar usuarios." });

            // Verificar si el RoleId solicitado existe
            var roleEntity = await _context.Roles.FindAsync(request.RoleId);
            if (roleEntity == null)
                return BadRequest(new { message = "Rol no válido. RoleId no encontrado en la base de datos." });

            // Verificar si el email ya existe
            var existingUser = await _context.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == request.Email);
            if (existingUser)
                return BadRequest(new { message = "El email ya está registrado." });

            // Crear el usuario
            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Name = request.Name,
                RoleId = request.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado correctamente." });
        }
    }
}

