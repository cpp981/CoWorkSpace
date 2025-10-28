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
    [Route("api/v1/providers")]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;

        public ProviderController(IProviderService providerService)
        {
            _providerService = providerService;
        }

        [HttpGet("provider/admins")]
        [Authorize]
        public async Task<IActionResult> GetAdminsForProvider()
        {
            try
            {
                var result = await _providerService.GetAdminsForProviderAsync(User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { Message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { Message = ex.Message }); }
            catch (Exception) { return StatusCode(500, new { Message = ApiMessages.INTERNAL_SERVER_ERROR }); }
        }

        [HttpPost("{providerId}/admins")]
        [Authorize]
        public async Task<IActionResult> CreateAdmin(int providerId, [FromBody] RegisterRequestDTO request)
        {
            try
            {
                var result = await _providerService.CreateAdminAsync(providerId, request, User);
                return Ok(result);
            }
            catch (ArgumentException ex) { return BadRequest(new RegisterResponseDTO { Message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new RegisterResponseDTO { Message = ex.Message }); }
            catch (Exception) { return StatusCode(500, new RegisterResponseDTO { Message = ApiMessages.INTERNAL_SERVER_ERROR }); }
        }

        [HttpPut("{providerId}/admins/{adminId}")]
        [Authorize]
        public async Task<IActionResult> UpdateAdmin(int providerId, int adminId, [FromBody] UpdateAdminRequestDTO request)
        {
            try
            {
                var result = await _providerService.UpdateAdminAsync(providerId, adminId, request, User);
                return Ok(result);
            }
            catch (ArgumentException ex) { return BadRequest(new UpdateAdminResponseDTO { Message = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new UpdateAdminResponseDTO { Message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new UpdateAdminResponseDTO { Message = ex.Message }); }
            catch (Exception) { return StatusCode(500, new UpdateAdminResponseDTO { Message = ApiMessages.INTERNAL_SERVER_ERROR }); }
        }

        [HttpDelete("{providerId}/admins/{adminId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAdmin(int providerId, int adminId)
        {
            try
            {
                var result = await _providerService.DeleteAdminAsync(providerId, adminId, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { Message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { Message = ex.Message }); }
            catch (Exception) { return StatusCode(500, new { Message = ApiMessages.INTERNAL_SERVER_ERROR }); }
        }
    }
}
