using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.Services;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admins")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("{adminId}/spaces")]
        [Authorize]
        public async Task<IActionResult> GetAdminSpaces(int adminId)
        {
            try
            {
                var result = await _adminService.GetAdminSpacesAsync(adminId, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { Message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { Message = ex.Message }); }
            catch (Exception) { return StatusCode(500, new { Message = ApiMessages.INTERNAL_SERVER_ERROR }); }
        }

        [HttpGet("{adminId}/spaces/{spaceId}/bookings")]
        [Authorize]
        public async Task<IActionResult> GetSpaceBookings(int adminId, int spaceId)
        {
            try
            {
                var result = await _adminService.GetSpaceBookingsAsync(adminId, spaceId, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { Message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { Message = ex.Message }); }
            catch (Exception) { return StatusCode(500, new { Message = ApiMessages.INTERNAL_SERVER_ERROR }); }
        }

        [HttpGet("{adminId}/clients")]
        [Authorize]
        public async Task<IActionResult> GetClientsForAdmin(int adminId)
        {
            try
            {
                var result = await _adminService.GetClientsForAdminAsync(adminId, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { Message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { Message = ex.Message }); }
            catch (Exception) { return StatusCode(500, new { Message = ApiMessages.INTERNAL_SERVER_ERROR }); }
        }

        [HttpPut("{adminId}/spaces/{spaceId}/bookings/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> UpdateBooking(int adminId, int spaceId, int bookingId, [FromBody] UpdateBookingDTO dto)
        {
            try
            {
                var result = await _adminService.UpdateBookingAsync(adminId, spaceId, bookingId, dto, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { Message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { Message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (Exception) { return StatusCode(500, new { Message = ApiMessages.INTERNAL_SERVER_ERROR }); }
        }

        [HttpDelete("{adminId}/spaces/{spaceId}/bookings/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> DeleteBooking(int adminId, int spaceId, int bookingId)
        {
            try
            {
                await _adminService.DeleteBookingAsync(adminId, spaceId, bookingId, User);
                return Ok(new { Message = ApiMessages.BOOKING_DELETED_SUCCESS });
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { Message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { Message = ex.Message }); }
            catch (Exception) { return StatusCode(500, new { Message = ApiMessages.INTERNAL_SERVER_ERROR }); }
        }
    }
}

