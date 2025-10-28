using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Constants;

namespace CoWorkSpace.Api.Services
{
    public class StatsService : IStatsService
    {
        private readonly CoWorkSpaceContext _context;

        public StatsService(CoWorkSpaceContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private (int userId, int roleId) ExtractIds(ClaimsPrincipal user)
        {
            var userId = int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : 0;
            var roleId = int.TryParse(user.FindFirst("roleId")?.Value, out var rid) ? rid : 0;
            return (userId, roleId);
        }

        public async Task<SuperAdminStatsDTO> GetSuperAdminStatsAsync(int idUser, ClaimsPrincipal user)
        {
            var (userId, roleId) = ExtractIds(user);
            if (userId != idUser) throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);
            if (roleId != 1) throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);

            var stats = new SuperAdminStatsDTO
            {
                TotalSpaces = await _context.Spaces.CountAsync(s => !s.IsDeleted),
                TotalBookings = await _context.Bookings.CountAsync(),
                TotalRevenue = await _context.Payments.SumAsync(p => p.Amount),
                TotalUsers = await _context.Users.CountAsync(),
                UsersByRole = await _context.Users
                    .GroupBy(u => u.Role.RoleName)
                    .Select(g => new { Role = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(k => k.Role, v => v.Count)
            };

            return stats;
        }

        public async Task<AdminStatsDTO> GetAdminStatsAsync(int idUser, ClaimsPrincipal user)
        {
            var (userId, roleId) = ExtractIds(user);
            if (userId != idUser) throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);
            if (roleId != 2) throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);

            var spaces = await _context.Spaces
                .Include(s => s.Bookings)
                .Include(s => s.Payments)
                .Include(s => s.Reviews)
                .Where(s => s.AdminId == userId && !s.IsDeleted)
                .ToListAsync();

            var spaceIds = spaces.Select(s => s.Id).ToList();

            var stats = new AdminStatsDTO
            {
                TotalSpaces = spaces.Count,
                TotalBookings = await _context.Bookings.CountAsync(b => spaceIds.Contains(b.SpaceId)),
                TotalRevenue = await _context.Payments
                    .Where(p => p.SpaceId.HasValue && spaceIds.Contains(p.SpaceId.Value))
                    .SumAsync(p => p.Amount),
                AverageRating = await _context.Reviews
                    .Where(r => spaceIds.Contains(r.SpaceId))
                    .AverageAsync(r => (double?)r.Rating) ?? 0.0,
                Spaces = spaces.Select(s => new SpaceStatsDTO
                {
                    SpaceId = s.Id,
                    SpaceName = s.Name,
                    BookingsCount = s.Bookings.Count,
                    Revenue = s.Payments.Sum(p => p.Amount),
                    AverageRating = s.Reviews.Any() ? s.Reviews.Average(r => (double)r.Rating) : 0.0
                }).ToList()
            };

            return stats;
        }

        public async Task<ProviderStatsDTO> GetProviderStatsAsync(int idUser, ClaimsPrincipal user)
        {
            var (userId, roleId) = ExtractIds(user);
            if (userId != idUser) throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);
            if (roleId != 3) throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);

            var spaces = await _context.Spaces
                .Include(s => s.Bookings)
                .Include(s => s.Payments)
                .Include(s => s.Admin)
                .Where(s => s.ProviderId == userId && !s.IsDeleted)
                .ToListAsync();

            var spaceIds = spaces.Select(s => s.Id).ToList();

            var stats = new ProviderStatsDTO
            {
                TotalSpaces = spaces.Count,
                TotalAdmins = await _context.Users.CountAsync(u => u.ProviderId == userId && u.RoleId == 2),
                TotalBookings = await _context.Bookings.CountAsync(b => spaceIds.Contains(b.SpaceId)),
                TotalRevenue = await _context.Payments
                    .Where(p => p.SpaceId.HasValue && spaceIds.Contains(p.SpaceId.Value))
                    .SumAsync(p => p.Amount),
                Spaces = spaces.Select(s => new ProviderSpaceStatsDTO
                {
                    SpaceId = s.Id,
                    SpaceName = s.Name,
                    AdminId = s.AdminId,
                    AdminName = s.Admin?.Name ?? string.Empty,
                    BookingsCount = s.Bookings.Count,
                    Revenue = s.Payments.Sum(p => p.Amount)
                }).ToList()
            };

            return stats;
        }

        public async Task<ClientStatsDTO> GetClientStatsAsync(int idUser, ClaimsPrincipal user)
        {
            var (userId, roleId) = ExtractIds(user);
            if (userId != idUser) throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);
            if (roleId != 4) throw new UnauthorizedAccessException(ApiMessages.UNAUTHORIZED);

            var bookings = await _context.Bookings
                .Include(b => b.Space)
                .Include(b => b.Payments)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            var stats = new ClientStatsDTO
            {
                TotalBookings = bookings.Count,
                TotalSpent = await _context.Payments.Where(p => p.UserId == userId).SumAsync(p => p.Amount),
                TotalReviews = await _context.Reviews.CountAsync(r => r.UserId == userId),
                Bookings = bookings.Select(b => new BookingStatsDTO
                {
                    BookingId = b.Id,
                    SpaceId = b.SpaceId,
                    Name = b.Space?.Name ?? string.Empty,
                    StartTime = b.StartTime,
                    Amount = b.Payments.Sum(p => p.Amount)
                }).ToList()
            };

            return stats;
        }
    }
}
