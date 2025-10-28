using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Services;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/v1/spaces/{spaceId}/bookings")]
    [Authorize]
    public class SpaceBookingsController : ControllerBase
    {
        private readonly ISpaceBookingService _service;

        public SpaceBookingsController(ISpaceBookingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingsForSpace(int spaceId)
        {
            try
            {
                var result = await _service.GetBookingsForSpaceAsync(spaceId, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = ApiMessages.INTERNAL_SERVER_ERROR });
            }
        }
    }
}
