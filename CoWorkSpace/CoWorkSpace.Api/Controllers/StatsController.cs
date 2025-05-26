using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Services;

namespace CoWorkSpace.Api.Controllers
{
    [Route("api/v1/stats")]
    [ApiController]
    [Authorize]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        // SuperAdmin: Total de usuarios y por rol
        [HttpGet("users")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetUserStats()
        {
            try
            {
                var stats = await _statsService.GetUserStats();
                return Ok(new
                {
                    totalUsers = stats.TotalUsers,
                    byRole = stats.UsersByRole
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new 
                { 
                    message = ApiMessages.ServerError 
                });
            }
        }

        // SuperAdmin: Registros por mes
        [HttpGet("registrations")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetRegistrationStats()
        {
            try
            {
                var stats = await _statsService.GetRegistrationStats();
                return Ok(new { registrations = stats });
            }
            catch (Exception)
            {
                return StatusCode(500, new 
                { 
                    message = ApiMessages.ServerError 
                });
            }
        }

        // SuperAdmin: Espacios totales y por proveedor
        [HttpGet("spaces")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetSpaceStats()
        {
            try
            {
                var stats = await _statsService.GetSpaceStats();
                return Ok(new
                {
                    totalSpaces = stats.TotalSpaces,
                    byProvider = stats.SpacesByProvider
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new 
                { 
                    message = ApiMessages.ServerError 
                });
            }
        }

        // Admin: Usuarios gestionados
        [HttpGet("admins/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminUserStats()
        {
            try
            {
                var stats = await _statsService.GetAdminUserStats();
                return Ok(new
                {
                    totalUsers = stats.TotalUsers
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new 
                { 
                    message = ApiMessages.ServerError 
                });
            }
        }

        // Admin: Reservas asociadas
        [HttpGet("admins/bookings")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminBookingStats()
        {
            try
            {
                var stats = await _statsService.GetAdminBookingStats();
                return Ok(new
                {
                    totalBookings = stats.TotalBookings,
                    byMonth = stats.BookingsByMonth
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new 
                { 
                    message = ApiMessages.ServerError 
                });
            }
        }

        // Provider: Espacios propios
        [HttpGet("providers/{providerId}/spaces")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> GetProviderSpaceStats(int providerId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!await _statsService.IsProviderAuthorized(userId, providerId))
            {
                return Unauthorized(new 
                { 
                    message = ApiMessages.Unauthorized 
                });
            }
            try
            {
                var stats = await _statsService.GetProviderSpaceStats(providerId);
                return Ok(new
                {
                    totalSpaces = stats.TotalSpaces
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new 
                { 
                    message = ApiMessages.ServerError 
                });
            }
        }

        // Provider: Reservas de sus espacios
        [HttpGet("providers/{providerId}/bookings")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> GetProviderBookingStats(int providerId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!await _statsService.IsProviderAuthorized(userId, providerId))
            {
                return Unauthorized(new 
                { 
                    message = ApiMessages.Unauthorized 
                });
            }
            try
            {
                var stats = await _statsService.GetProviderBookingStats(providerId);
                return Ok(new
                {
                    totalBookings = stats.TotalBookings,
                    byDay = stats.BookingsByDay
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new 
                { 
                    message = ApiMessages.ServerError 
                });
            }
        }

        // Provider: Ocupación de espacios
        [HttpGet("providers/{providerId}/occupancy")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> GetProviderOccupancyStats(int providerId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!await _statsService.IsProviderAuthorized(userId, providerId))
            {
                return Unauthorized(new 
                { 
                    message = ApiMessages.Unauthorized 
                });
            }
            try
            {
                var stats = await _statsService.GetProviderOccupancyStats(providerId);
                return Ok(new
                {
                    occupancyRate = stats.OccupancyRate,
                    byDay = stats.OccupancyByDay
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new 
                { 
                    message = ApiMessages.ServerError 
                });
            }
        }

        // Client: Reservas propias
        [HttpGet("clients/{userId}/bookings")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetClientBookingStats(string userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != userId)
            {
                return Unauthorized(new 
                { 
                    message = ApiMessages.Unauthorized 
                });
            }
            try
            {
                var stats = await _statsService.GetClientBookingStats(userId);
                return Ok(new
                {
                    totalBookings = stats.TotalBookings,
                    bookings = stats.Bookings
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new 
                { 
                    message = ApiMessages.ServerError 
                });
            }
        }

        // Client: Actividad reciente
        [HttpGet("clients/{userId}/activity")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetClientActivityStats(string userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != userId)
            {
                return Unauthorized(new 
                { 
                    message = ApiMessages.Unauthorized 
                });
            }
            try
            {
                var stats = await _statsService.GetClientActivityStats(userId);
                return Ok(new
                {
                    totalActions = stats.TotalActions,
                    actions = stats.Actions
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new 
                { 
                    message = ApiMessages.ServerError 
                });
            }
        }
    }
}