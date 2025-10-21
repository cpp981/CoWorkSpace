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
    [Route("api/v1/admins")]
    public class AdminController : Controller
    {
        private readonly CoWorkSpaceContext _context;
        public AdminController(CoWorkSpaceContext context)
        {
            _context = context;
        }

        [HttpGet("{adminId}/spaces")]
        [Authorize]
        public async Task<IActionResult> GetAdminSpaces(int adminId)
        {
            // Obtener claims del usuario autenticado
            var roleIdClaim = User.FindFirst("roleId")?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(roleIdClaim) || string.IsNullOrEmpty(userIdClaim)
                || !int.TryParse(roleIdClaim, out int roleId)
                || !int.TryParse(userIdClaim, out int loggedUserId))
            {
                return Unauthorized(new { Message = ApiMessages.UNAUTHORIZED });
            }

            // Solo Admins (rol 2) pueden usar este endpoint
            if (roleId != 2)
                return Unauthorized(new { Message = ApiMessages.ONLY_ADMINS_CAN_ACCESS_SPACES });

            // El admin solo puede ver sus propios espacios
            if (loggedUserId != adminId)
                return Unauthorized(new { Message = ApiMessages.CANNOT_ACCESS_OTHER_ADMIN_SPACE });

            // Traer todos los espacios del admin primero
            var spaces = await _context.Spaces
                .Where(s => s.AdminId == adminId)
                .ToListAsync();

            if (!spaces.Any())
            {
                return NotFound(new { Message = ApiMessages.NO_SPACES_FOUND_FOR_ADMIN });
            }

            // Proyectar manualmente para poder incluir la siguiente reserva correctamente
            var result = spaces.Select(s =>
            {
                var nextBooking = _context.Bookings
                    .Where(b => b.SpaceId == s.Id && b.StartTime > DateTime.UtcNow)
                    .OrderBy(b => b.StartTime)
                    .Select(b => (DateTime?)b.StartTime)
                    .FirstOrDefault();

                var estado = _context.Bookings.Any(b =>
                    b.SpaceId == s.Id &&
                    b.StartTime <= DateTime.UtcNow &&
                    b.EndTime >= DateTime.UtcNow
                ) ? "Ocupado" : "Libre";

                return new AdminSpacesResponseDTO
                {
                    Id = s.Id,
                    Nombre = s.Name,
                    Ciudad = s.City,
                    Precio = s.Price,
                    Estado = estado,
                    SiguienteReserva = nextBooking?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty
                };
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{adminId}/spaces/{spaceId}/bookings")]
        [Authorize]
        public async Task<IActionResult> GetSpaceBookings(int adminId, int spaceId)
        {
            // Claims del usuario logueado
            var roleIdClaim = User.FindFirst("roleId")?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(roleIdClaim) || string.IsNullOrEmpty(userIdClaim)
                || !int.TryParse(roleIdClaim, out int roleId)
                || !int.TryParse(userIdClaim, out int loggedUserId))
            {
                return Unauthorized(new { Message = ApiMessages.UNAUTHORIZED });
            }

            if (roleId != 2)
                return Unauthorized(new { Message = ApiMessages.ONLY_ADMINS_CAN_ACCESS_SPACES });

            if (loggedUserId != adminId)
                return Unauthorized(new { Message = ApiMessages.CANNOT_ACCESS_OTHER_ADMIN_SPACE });

            // Verificar que el espacio pertenece a este admin
            var space = await _context.Spaces.FirstOrDefaultAsync(s => s.Id == spaceId && s.AdminId == adminId);
            if (space == null)
                return NotFound(new { Message = ApiMessages.NO_SPACES_FOUND_FOR_ADMIN });

            // Traer reservas + nombre cliente
            var bookings = await _context.Bookings
                .Where(b => b.SpaceId == spaceId)
                .Join(_context.Users,
                    b => b.UserId,
                    u => u.Id,
                    (b, u) => new
                    {
                        b.Id,
                        b.UserId,
                        NombreCliente = u.Name,
                        FechaInicio = b.StartTime,
                        FechaFin = b.EndTime
                    })
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpGet("{adminId}/clients")]
        [Authorize]
        public async Task<IActionResult> GetClientsForAdmin(int adminId)
        {
            // Obtener claims del usuario autenticado
            var roleIdClaim = User.FindFirst("roleId")?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(roleIdClaim) || string.IsNullOrEmpty(userIdClaim)
                || !int.TryParse(roleIdClaim, out int roleId)
                || !int.TryParse(userIdClaim, out int loggedUserId))
            {
                return Unauthorized(new { Message = ApiMessages.UNAUTHORIZED });
            }

            // Solo Admins (rol 2) pueden usar este endpoint
            if (roleId != 2)
                return Unauthorized(new { Message = ApiMessages.ONLY_ADMINS_CAN_ACCESS_SPACES });

            // El admin solo puede consultar sus propios clientes
            if (loggedUserId != adminId)
                return Unauthorized(new { Message = ApiMessages.CANNOT_ACCESS_OTHER_ADMINS_CLIENTS });

            // Consulta: unir Bookings -> Spaces -> Users filtrando por espacios del admin
            var query = from b in _context.Bookings
                        join s in _context.Spaces on b.SpaceId equals s.Id
                        join u in _context.Users on b.UserId equals u.Id
                        where s.AdminId == adminId
                        select new
                        {
                            ClientId = u.Id,
                            ClientName = u.Name,
                            SpaceName = s.Name
                        };

            var grouped = await query
                .AsNoTracking()
                .GroupBy(x => new { x.ClientId, x.ClientName })
                .Select(g => new ClientWithSpacesDTO
                {
                    ClientId = g.Key.ClientId,
                    ClientName = g.Key.ClientName,
                    SpaceNames = g.Select(x => x.SpaceName).Distinct().ToList()
                })
                .ToListAsync();

            if (!grouped.Any())
                return NotFound(new { Message = ApiMessages.NO_CLIENTS_FOUND_FOR_ADMIN });

            return Ok(grouped);
        }
    }
}
