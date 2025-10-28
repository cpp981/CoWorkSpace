using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using CoWorkSpace.Api.Constants;
using CoWorkSpace.Api.DTOs;
using CoWorkSpace.Api.Services;

namespace CoWorkSpace.Api.Controllers
{
    [Route("api/v1/stats")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _service;

        public StatsController(IStatsService service)
        {
            _service = service;
        }

        [HttpGet("superadmin/{idUser}")]
        [Authorize]
        public async Task<ActionResult<SuperAdminStatsDTO>> GetSuperAdminStats(int idUser)
        {
            try
            {
                var result = await _service.GetSuperAdminStatsAsync(idUser, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (Exception) { return StatusCode(500, ApiMessages.INTERNAL_SERVER_ERROR); }
        }

        [HttpGet("admin/{idUser}")]
        [Authorize]
        public async Task<ActionResult<AdminStatsDTO>> GetAdminStats(int idUser)
        {
            try
            {
                var result = await _service.GetAdminStatsAsync(idUser, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (Exception) { return StatusCode(500, ApiMessages.INTERNAL_SERVER_ERROR); }
        }

        [HttpGet("provider/{idUser}")]
        [Authorize]
        public async Task<ActionResult<ProviderStatsDTO>> GetProviderStats(int idUser)
        {
            try
            {
                var result = await _service.GetProviderStatsAsync(idUser, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (Exception) { return StatusCode(500, ApiMessages.INTERNAL_SERVER_ERROR); }
        }

        [HttpGet("client/{idUser}")]
        [Authorize]
        public async Task<ActionResult<ClientStatsDTO>> GetClientStats(int idUser)
        {
            try
            {
                var result = await _service.GetClientStatsAsync(idUser, User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (Exception) { return StatusCode(500, ApiMessages.INTERNAL_SERVER_ERROR); }
        }
    }
}
