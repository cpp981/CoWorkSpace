using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Models;

namespace CoWorkSpace.Api.Services
{
    public class AdminService : IAdminService
    {
        private readonly CoWorkSpaceContext _context;

        public AdminService(CoWorkSpaceContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private (int roleId, int userId) ExtractClaims(ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst("roleId")?.Value;
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(roleClaim) || string.IsNullOrEmpty(idClaim)
                || !int.TryParse(roleClaim, out int roleId)
                || !int.TryParse(idClaim, out int userId))
            {
                throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);
            }

            return (roleId, userId);
        }

        public async Task<List<AdminSpacesResponseDTO>> GetAdminSpacesAsync(int adminId, ClaimsPrincipal user)
        {
            var (roleId, loggedUserId) = ExtractClaims(user);

            if (roleId != 2) throw new UnauthorizedAccessException(ApiMessages.ONLY_ADMINS_CAN_ACCESS_SPACES);
            if (loggedUserId != adminId) throw new UnauthorizedAccessException(ApiMessages.CANNOT_ACCESS_OTHER_ADMIN_SPACE);

            var spaces = await _context.Spaces
                .Where(s => s.AdminId == adminId)
                .ToListAsync();

            if (!spaces.Any()) throw new KeyNotFoundException(ApiMessages.NO_SPACES_FOUND_FOR_ADMIN);

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

            return result;
        }

        public async Task<List<BookingForAdminDTO>> GetSpaceBookingsAsync(int adminId, int spaceId, ClaimsPrincipal user)
        {
            var (roleId, loggedUserId) = ExtractClaims(user);

            if (roleId != 2) throw new UnauthorizedAccessException(ApiMessages.ONLY_ADMINS_CAN_ACCESS_SPACES);
            if (loggedUserId != adminId) throw new UnauthorizedAccessException(ApiMessages.CANNOT_ACCESS_OTHER_ADMIN_SPACE);

            var space = await _context.Spaces.FirstOrDefaultAsync(s => s.Id == spaceId && s.AdminId == adminId);
            if (space == null) throw new KeyNotFoundException(ApiMessages.NO_SPACES_FOUND_FOR_ADMIN);

            var bookings = await _context.Bookings
                .Where(b => b.SpaceId == spaceId)
                .Join(_context.Users,
                    b => b.UserId,
                    u => u.Id,
                    (b, u) => new BookingForAdminDTO
                    {
                        Id = b.Id,
                        UserId = b.UserId,
                        NombreCliente = u.Name,
                        FechaInicio = b.StartTime,
                        FechaFin = b.EndTime
                    })
                .ToListAsync();

            return bookings;
        }

        public async Task<List<ClientWithSpacesDTO>> GetClientsForAdminAsync(int adminId, ClaimsPrincipal user)
        {
            var (roleId, loggedUserId) = ExtractClaims(user);

            if (roleId != 2) throw new UnauthorizedAccessException(ApiMessages.ONLY_ADMINS_CAN_ACCESS_SPACES);
            if (loggedUserId != adminId) throw new UnauthorizedAccessException(ApiMessages.CANNOT_ACCESS_OTHER_ADMINS_CLIENTS);

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

            if (!grouped.Any()) throw new KeyNotFoundException(ApiMessages.NO_CLIENTS_FOUND_FOR_ADMIN);

            return grouped;
        }

        public async Task<BookingUpdateResultDTO> UpdateBookingAsync(int adminId, int spaceId, int bookingId, UpdateBookingDTO dto, ClaimsPrincipal user)
        {
            var (roleId, loggedUserId) = ExtractClaims(user);

            if (roleId != 2) throw new UnauthorizedAccessException(ApiMessages.ONLY_ADMINS_CAN_ACCESS_SPACES);
            if (loggedUserId != adminId) throw new UnauthorizedAccessException(ApiMessages.CANNOT_ACCESS_OTHER_ADMIN_SPACE);

            var space = await _context.Spaces.AsNoTracking().FirstOrDefaultAsync(s => s.Id == spaceId && s.AdminId == adminId);
            if (space == null) throw new KeyNotFoundException(ApiMessages.NO_SPACES_FOUND_FOR_ADMIN);

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.SpaceId == spaceId);
            if (booking == null) throw new KeyNotFoundException(ApiMessages.BOOKING_NOT_FOUND_FOR_SPACE);

            if (dto == null || string.IsNullOrEmpty(dto.Start) || string.IsNullOrEmpty(dto.End))
                throw new ArgumentException(ApiMessages.START_DATE_AND_END_DATE_ARE_REQUIREDS);

            if (!DateTime.TryParse(dto.Start, out DateTime newStart) || !DateTime.TryParse(dto.End, out DateTime newEnd))
                throw new ArgumentException(ApiMessages.INVALID_DATE_FORMAT);

            if (newStart >= newEnd)
                throw new ArgumentException(ApiMessages.START_DATE_BEFORE_END_DATE);

            booking.StartTime = newStart;
            booking.EndTime = newEnd;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            return new BookingUpdateResultDTO
            {
                Id = booking.Id,
                UserId = booking.UserId,
                Start = booking.StartTime.ToString("o"),
                End = booking.EndTime.ToString("o"),
                Message = ApiMessages.BOOKING_UPDATED_SUCCESS
            };
        }

        public async Task DeleteBookingAsync(int adminId, int spaceId, int bookingId, ClaimsPrincipal user)
        {
            var (roleId, loggedUserId) = ExtractClaims(user);

            if (roleId != 2) throw new UnauthorizedAccessException(ApiMessages.ONLY_ADMINS_CAN_ACCESS_SPACES);
            if (loggedUserId != adminId) throw new UnauthorizedAccessException(ApiMessages.CANNOT_ACCESS_OTHER_ADMIN_SPACE);

            var space = await _context.Spaces.AsNoTracking().FirstOrDefaultAsync(s => s.Id == spaceId && s.AdminId == adminId);
            if (space == null) throw new KeyNotFoundException(ApiMessages.NO_SPACES_FOUND_FOR_ADMIN);

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.SpaceId == spaceId);
            if (booking == null) throw new KeyNotFoundException(ApiMessages.BOOKING_NOT_FOUND_FOR_SPACE);

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}
