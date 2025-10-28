using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.DTOs.Provider;
using CoWorkSpace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CoWorkSpace.Api.Controllers
{
    [ApiController]
    [Route("api/v1/providers/{providerId}/spaces")]
    [Authorize]
    public class ProviderSpacesController : ControllerBase
    {
        private readonly IProviderSpaceService _service;

        public ProviderSpacesController(IProviderSpaceService service)
        {
            _service = service;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetSpacesByProvider(int providerId)
        {
            try
            {
                var result = await _service.GetSpacesByProviderAsync(providerId, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception) { return StatusCode(500, ApiMessages.INTERNAL_SERVER_ERROR); }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditSpace(int providerId, int id, [FromBody] SpaceCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return StatusCode(403, new { success = false, message = ApiMessages.INVALID_DATA });

            try
            {
                var updatedId = await _service.EditSpaceAsync(providerId, id, dto, User);
                return Ok(new { success = true, message = ApiMessages.SPACE_UPDATED_SUCCESS, id = updatedId });
            }
            catch (UnauthorizedAccessException) { return StatusCode(403, new { success = false, message = ApiMessages.NO_PERMISSION_UPDATE_SPACE }); }
            catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
            catch (ArgumentException) { return StatusCode(403, new { success = false, message = ApiMessages.INVALID_DATA }); }
            catch (Exception) { return StatusCode(500, new { success = false, message = ApiMessages.SPACE_UPDATED_ERROR }); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpace(int providerId, int id)
        {
            try
            {
                await _service.DeleteSpaceAsync(providerId, id, User);
                return Ok(new { success = true, message = ApiMessages.SPACE_DELETED_SUCCESS });
            }
            catch (UnauthorizedAccessException) { return StatusCode(403, new { success = false, message = ApiMessages.NO_PERMISSION_DELETE_SPACE }); }
            catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
            catch (Exception) { return StatusCode(500, new { success = false, message = ApiMessages.SPACE_DELETED_ERROR }); }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSpace(int providerId, [FromBody] SpaceCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return StatusCode(403, new { success = false, message = ApiMessages.INVALID_DATA });

            try
            {
                var newId = await _service.CreateSpaceAsync(providerId, dto, User);
                return Ok(new { success = true, message = ApiMessages.SPACE_CREATED_SUCCESS, spaceId = newId });
            }
            catch (UnauthorizedAccessException) { return StatusCode(403, new { success = false, message = ApiMessages.INVALID_DATA }); }
            catch (ArgumentException) { return StatusCode(403, new { success = false, message = ApiMessages.INVALID_DATA }); }
            catch (Exception) { return StatusCode(500, new { success = false, message = ApiMessages.SPACE_CREATED_ERROR }); }
        }
    }
}
