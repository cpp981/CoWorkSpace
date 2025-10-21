using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/v1/spaces/{spaceId}/bookings")]
    [Authorize]
    public class SpaceBookingsController : ControllerBase
    {
        private readonly CoWorkSpaceContext _context;

        public SpaceBookingsController(CoWorkSpaceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingsForSpace(int spaceId)
        {
            int providerId = GetIntClaim(ClaimTypes.NameIdentifier);
            if (providerId == 0)
                providerId = GetIntClaim(JwtRegisteredClaimNames.Sub);

            int roleId = GetIntClaim("roleId");

            // Solo providers (roleId = 3)
            if (roleId != 3)
                return Unauthorized(new {Message = ApiMessages.UNAUTHORIZED});

            // Comprobamos que el espacio pertenece al provider logueado
            var space = await _context.Spaces
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == spaceId && s.ProviderId == providerId);

            if (space == null)
                return Unauthorized(new { Message = ApiMessages.NO_SPACES_FOR_PROVIDER });

            // Traemos las reservas con nombre de usuario y espacio
            var bookings = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.SpaceId == spaceId)
                .Include(b => b.User)
                .Select(b => new BookingResponseDTO
                {
                    Id = b.Id,
                    SpaceId = b.SpaceId,
                    SpaceName = space.Name,
                    UserId = b.UserId,
                    UserName = b.User != null ? b.User.Name : string.Empty,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime
                })
                .ToListAsync();

            return Ok(bookings);
        }

        private int GetIntClaim(string claimName)
        {
            var claim = User.FindFirst(claimName)?.Value;
            if (string.IsNullOrEmpty(claim))
                return 0;
            return int.TryParse(claim, out var v) ? v : 0;
        }
    }
}
