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
    [Route("api/v1/providers")]
    public class ProviderController : Controller
    {
        private readonly CoWorkSpaceContext _context;

        public ProviderController(CoWorkSpaceContext context)
        {
            _context = context;
        }

        [HttpGet("provider/admins")]
        [Authorize]
        public async Task<IActionResult> GetAdminsForProvider()
        {
            // Obtener claims del usuario autenticado
            var roleIdClaim = User.FindFirst("roleId")?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(roleIdClaim) || string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { Message = ApiMessages.UNAUTHORIZED });

            if (!int.TryParse(roleIdClaim, out int roleId) || !int.TryParse(userIdClaim, out int loggedUserId))
                return Unauthorized(new { Message = ApiMessages.INVALID_USER });

            // Solo Providers (rol 3) pueden acceder
            if (roleId != 3)
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { Message = ApiMessages.ONLY_PROVIDERS_CAN_VIEW_ADMINS });

            // Obtener admins que pertenezcan al provider autenticado
            var admins = await _context.Users
                .Where(u => u.RoleId == 2 && u.ProviderId == loggedUserId)
                .Select(u => new
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                })
                .ToListAsync();

            if (!admins.Any())
                return NotFound(new { Message = ApiMessages.NO_ADMINS_FOUND });

            return Ok(admins);
        }

        [HttpPost("{providerId}/admins")]
        [Authorize]
        public async Task<IActionResult> CreateAdmin(int providerId, [FromBody] RegisterRequestDTO request)
        {
            // Validación manual de la contraseña (antes del ModelState)
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            {
                return BadRequest(new RegisterResponseDTO
                {
                    Message = ApiMessages.PASSWORD_TOO_SHORT
                });
            }

            // Validar otros campos por atributos [Required]
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegisterResponseDTO
                {
                    Message = ApiMessages.INVALID_DATA
                });
            }

            // Obtener claims de rol y usuario
            var roleIdClaim = User.FindFirst("roleId")?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (roleIdClaim == null || userIdClaim == null || !int.TryParse(roleIdClaim, out int roleId))
                return Unauthorized(new RegisterResponseDTO
                {
                    Message = ApiMessages.UNAUTHORIZED
                });

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new RegisterResponseDTO
                {
                    Message = ApiMessages.INVALID_USER
                });

            // Solo Providers (rol 3) pueden usar este endpoint
            if (roleId != 3)
                return Unauthorized(new RegisterResponseDTO
                {
                    Message = ApiMessages.ONLY_PROVIDERS_CAN_CREATE_ADMINS
                });

            // Solo puede crear admins para su propio providerId
            if (userId != providerId)
                return Unauthorized(new RegisterResponseDTO
                {
                    Message = ApiMessages.CANNOT_CREATE_ADMINS_FOR_OTHER_PROVIDERS
                });

            // Validar que se está creando un admin (roleId = 2)
            if (request.RoleId != 2)
                return BadRequest(new RegisterResponseDTO
                {
                    Message = ApiMessages.ONLY_ADMIN_ROLE_ALLOWED
                });

            // Verificar si el email ya existe
            var exists = await _context.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == request.Email);
            if (exists)
                return BadRequest(new RegisterResponseDTO
                {
                    Message = ApiMessages.EMAIL_ALREADY_REGISTERED
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
                Message = ApiMessages.ADMIN_CREATED_SUCCESS
            });
        }

        [HttpPut("{providerId}/admins/{adminId}")]
        [Authorize]
        public async Task<IActionResult> UpdateAdmin(int providerId, int adminId, [FromBody] UpdateAdminRequestDTO request)
        {
            // Obtener claims del usuario autenticado
            var roleIdClaim = User.FindFirst("roleId")?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(roleIdClaim) || string.IsNullOrEmpty(userIdClaim)
                || !int.TryParse(roleIdClaim, out int roleId)
                || !int.TryParse(userIdClaim, out int loggedUserId))
            {
                return Unauthorized(new UpdateAdminResponseDTO
                {
                    Message = ApiMessages.UNAUTHORIZED
                });
            }

            // Solo Providers (rol 3) pueden usar este endpoint
            if (roleId != 3)
                return Unauthorized(new UpdateAdminResponseDTO
                {
                    Message = ApiMessages.ONLY_PROVIDERS_CAN_EDIT_ADMINS
                });

            // El provider solo puede editar sus propios admins
            if (loggedUserId != providerId)
                return Unauthorized(new UpdateAdminResponseDTO
                {
                    Message = ApiMessages.CANNOT_EDIT_ADMINS_FOR_OTHER_PROVIDERS
                });

            // Buscar el admin a editar
            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == adminId && u.ProviderId == providerId && u.RoleId == 2);

            if (adminUser == null)
                return NotFound(new UpdateAdminResponseDTO
                {
                    Message = ApiMessages.ADMIN_NOT_FOUND
                });

            // Validar si se cambia el email
            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != adminUser.Email)
            {
                var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
                if (exists)
                {
                    return BadRequest(new UpdateAdminResponseDTO
                    {
                        Message = ApiMessages.EMAIL_ALREADY_REGISTERED
                    });
                }

                adminUser.Email = request.Email;
            }

            // Actualizar campos
            adminUser.Name = request.Name ?? adminUser.Name;

            // Si se manda contraseña nueva, validar longitud y hashearla
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                if (request.Password.Length < 6)
                {
                    return BadRequest(new UpdateAdminResponseDTO
                    {
                        Message = ApiMessages.PASSWORD_TOO_SHORT
                    });
                }

                adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _context.SaveChangesAsync();

            return Ok(new UpdateAdminResponseDTO
            {
                Id = adminUser.Id,
                Name = adminUser.Name,
                Email = adminUser.Email,
                ProviderId = (int)adminUser.ProviderId,
                RoleId = adminUser.RoleId,
                Message = ApiMessages.ADMIN_UPDATED_SUCCESS
            });
        }

        [HttpDelete("{providerId}/admins/{adminId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAdmin(int providerId, int adminId)
        {
            var roleIdClaim = User.FindFirst("roleId")?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(roleIdClaim) || string.IsNullOrEmpty(userIdClaim)
                || !int.TryParse(roleIdClaim, out int roleId)
                || !int.TryParse(userIdClaim, out int loggedUserId))
            {
                return Unauthorized(new { Message = ApiMessages.UNAUTHORIZED });
            }

            if (roleId != 3)
                return Forbid();

            if (loggedUserId != providerId)
                return Unauthorized(new { Message = ApiMessages.CANNOT_DELETE_ADMINS_FOR_OTHER_PROVIDERS });

            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == adminId && u.ProviderId == providerId && u.RoleId == 2);

            if (adminUser == null)
                return NotFound(new { Message = ApiMessages.ADMIN_NOT_FOUND });

            _context.Users.Remove(adminUser);
            await _context.SaveChangesAsync();

            var response = new DeleteAdminResponseDto
            {
                Id = adminUser.Id,
                Name = adminUser.Name,
                Email = adminUser.Email,
                Message = ApiMessages.ADMIN_DELETED_SUCCESS
            };

            return Ok(response);
        }
    }
}