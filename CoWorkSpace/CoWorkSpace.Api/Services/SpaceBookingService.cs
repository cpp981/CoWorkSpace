using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;

namespace CoWorkSpace.Api.Services
{
    public class SpaceBookingService : ISpaceBookingService
    {
        private readonly CoWorkSpaceContext _context;

        public SpaceBookingService(CoWorkSpaceContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private int GetIntClaim(ClaimsPrincipal user, string claimName)
        {
            var claim = user.FindFirst(claimName)?.Value;
            if (string.IsNullOrEmpty(claim)) return 0;
            return int.TryParse(claim, out var v) ? v : 0;
        }

        public async Task<List<BookingResponseDTO>> GetBookingsForSpaceAsync(int spaceId, ClaimsPrincipal user)
        {
            int providerId = GetIntClaim(user, ClaimTypes.NameIdentifier);
            if (providerId == 0)
                providerId = GetIntClaim(user, System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

            int roleId = GetIntClaim(user, "roleId");

            if (roleId != 3) // Solo providers
                throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);

            var space = await _context.Spaces
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == spaceId && s.ProviderId == providerId);

            if (space == null)
                throw new UnauthorizedAccessException(ApiMessages.NO_SPACES_FOR_PROVIDER);

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

            return bookings;
        }
    }
}
