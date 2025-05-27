using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.DTOs;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace CoWorkSpace.Api.Controllers
{
    [Route("api/v1/stats")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly CoWorkSpaceContext _context;

        public StatsController(CoWorkSpaceContext context)
        {
            _context = context;
        }

        // GET: api/v1/stats/superadmin/{idUser}
        [HttpGet("superadmin/{idUser}")]
        [Authorize]
        public async Task<ActionResult<SuperAdminStatsDTO>> GetSuperAdminStats(int idUser)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var roleId = int.Parse(User.FindFirst("roleId")?.Value ?? "0");

            if (userId != idUser)
            {
                return Forbid();
            }

            if (roleId != 1) // SuperAdmin RoleId
            {
                return Forbid();
            }

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

            return Ok(stats);
        }

        // GET: api/v1/stats/admin/{idUser}
        [HttpGet("admin/{idUser}")]
        [Authorize]
        public async Task<ActionResult<AdminStatsDTO>> GetAdminStats(int idUser)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var roleId = int.Parse(User.FindFirst("roleId")?.Value ?? "0");

            if (userId != idUser)
            {
                return Forbid();
            }

            if (roleId != 2) // Admin RoleId
            {
                return Forbid();
            }

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
                TotalBookings = await _context.Bookings
                    .CountAsync(b => spaceIds.Contains(b.SpaceId)),
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

            return Ok(stats);
        }

        // GET: api/v1/stats/provider/{idUser}
        [HttpGet("provider/{idUser}")]
        [Authorize]
        public async Task<ActionResult<ProviderStatsDTO>> GetProviderStats(int idUser)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var roleId = int.Parse(User.FindFirst("roleId")?.Value ?? "0");

            if (userId != idUser)
            {
                return Forbid();
            }

            if (roleId != 3) // Provider RoleId
            {
                return Forbid();
            }

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
                TotalAdmins = await _context.Users
                    .CountAsync(u => u.ProviderId == userId && u.RoleId == 2),
                TotalBookings = await _context.Bookings
                    .CountAsync(b => spaceIds.Contains(b.SpaceId)),
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

            return Ok(stats);
        }

        // GET: api/v1/stats/client/{idUser}
        [HttpGet("client/{idUser}")]
        [Authorize]
        public async Task<ActionResult<ClientStatsDTO>> GetClientStats(int idUser)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var roleId = int.Parse(User.FindFirst("roleId")?.Value ?? "0");

            if (userId != idUser)
            {
                return Forbid();
            }

            if (roleId != 4) // Client RoleId
            {
                return Forbid();
            }

            var bookings = await _context.Bookings
                .Include(b => b.Space)
                .Include(b => b.Payments)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            var stats = new ClientStatsDTO
            {
                TotalBookings = bookings.Count,
                TotalSpent = await _context.Payments
                    .Where(p => p.UserId == userId)
                    .SumAsync(p => p.Amount),
                TotalReviews = await _context.Reviews
                    .CountAsync(r => r.UserId == userId),
                Bookings = bookings.Select(b => new BookingStatsDTO
                {
                    BookingId = b.Id,
                    SpaceId = b.SpaceId,
                    Name = b.Space?.Name ?? string.Empty,
                    StartTime = b.StartTime,
                    Amount = b.Payments.Sum(p => p.Amount)
                }).ToList()
            };

            return Ok(stats);
        }
    }
}