using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/providers")]
    public class ProviderController : Controller
    {
        private readonly CoWorkSpaceContext _context;

        public ProviderController(CoWorkSpaceContext context)
        {
            _context = context;
        }

        [HttpPost("{providerId}/admins")]
        [Authorize]
        public async Task<IActionResult> CreateAdmin(int providerId, [FromBody] RegisterRequestDTO request)
        {
            // Validar modelo
            if (!ModelState.IsValid)
                return BadRequest(new RegisterResponseDTO
                {
                    Message = ApiMessages.InvalidData
                });

            // Obtener claims de rol y usuario
            var roleIdClaim = User.FindFirst("roleId")?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (roleIdClaim == null || userIdClaim == null || !int.TryParse(roleIdClaim, out int roleId))
                return Unauthorized(new RegisterResponseDTO
                { 
                    Message = ApiMessages.Unauthorized 
                });

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new RegisterResponseDTO
                { 
                    Message = ApiMessages.InvalidUser 
                });

            // Solo Providers (rol 3) pueden usar este endpoint
            if (roleId != 3)
                return Unauthorized(new RegisterResponseDTO
                { 
                    Message = ApiMessages.OnlyProvidersCanCreateAdmins 
                });

            // Solo puede crear admins para su propio providerId
            if (userId != providerId)
                return Unauthorized(new RegisterResponseDTO
                { 
                    Message = ApiMessages.CannotCreateAdminsForOtherProviders 
                });

            // Validar que se está creando un admin (roleId = 2)
            if (request.RoleId != 2)
                return BadRequest(new RegisterResponseDTO
                { 
                    Message = ApiMessages.OnlyAdminRoleAllowed 
                });

            // Verificar si el email ya existe
            var exists = await _context.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == request.Email);
            if (exists)
                return BadRequest(new RegisterResponseDTO
                { 
                    Message = ApiMessages.EmailAlreadyRegistered 
                });

            // Crear nuevo usuario admin
            var adminUser = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Name = request.Name,
                RoleId = 2,
                ProviderId = providerId
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            return Ok(new RegisterResponseDTO 
            {
                Id = adminUser.Id,
                Email = adminUser.Email,
                Name = adminUser.Name,
                RoleId = adminUser.RoleId,
                ProviderId = adminUser.ProviderId,
                Message = ApiMessages.AdminCreatedSuccessfully 
            });
        }
    }
}