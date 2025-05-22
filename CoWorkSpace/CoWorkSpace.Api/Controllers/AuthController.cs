using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.Models;
using CoWorkSpace.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using CoWorkSpace.Api.Constants;

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
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new RegisterResponseDTO
                {
                    Message = ApiMessages.InvalidData
                });

            // Permitir solo roles 3 y 4 para registrar
            if (request.RoleId != 3 && request.RoleId != 4)
                return BadRequest(new RegisterResponseDTO
                {
                    Message = ApiMessages.RoleNotAllowedOnlyCanRegisterProviderOrClient
                });

            // Verificar si el RoleId solicitado existe
            var roleEntity = await _context.Roles.FindAsync(request.RoleId);
            if (roleEntity == null)
                return BadRequest(new RegisterResponseDTO
                {
                    Message = ApiMessages.InvalidRoleOrRoleIdNotFound
                });

            // Verificar si el email ya existe
            var existingUser = await _context.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == request.Email);
            if (existingUser)
                return BadRequest(new RegisterResponseDTO
                {
                    Message = ApiMessages.EmailAlreadyRegistered
                });

            // Crear el usuario
            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Name = request.Name,
                RoleId = request.RoleId,
                ProviderId = null // ProviderId no se usa para RoleId = 3 o 4
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new RegisterResponseDTO
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                RoleId = user.RoleId,
                ProviderId = user.ProviderId,
                Message = ApiMessages.UserRegisteredSuccessfully
            });
        }
    }
}